namespace NetworkedPlugins
{
    using Exiled.API.Features;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Packets.ClientPackets;
    using NetworkedPlugins.API.Models;
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
            writer.Put(Player.Nickname);
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


        public void SendPacket(NetDataWriter writer, Player plr, PlayerDataType dataType)
        {
            NPManager.Singleton.PacketProcessor.Send<UpdatePlayerInfoPacket>(
                NPManager.Singleton.NetworkListener,
                new UpdatePlayerInfoPacket()
                {
                    UserID = plr.UserId,
                    Type = (byte)dataType,
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
                SendPacket(writer, Player, PlayerDataType.Nickname);
            }

            if (NetworkData.Role != (PlayerRole)Player.Role && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRole))
            {
                NetworkData.Role = (PlayerRole)Player.Role;
                writer = new NetDataWriter();
                writer.Put((sbyte)NetworkData.Role);
                SendPacket(writer, Player, PlayerDataType.Role);
            }

            if (NetworkData.DoNotTrack != Player.DoNotTrack && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerDoNotTrack))
            {
                NetworkData.DoNotTrack = Player.DoNotTrack;
                writer = new NetDataWriter();
                writer.Put(NetworkData.DoNotTrack);
                SendPacket(writer, Player, PlayerDataType.DoNotTrack);
            }

            if (NetworkData.RemoteAdminAccess != Player.RemoteAdminAccess && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRemoteAdminAccess))
            {
                NetworkData.RemoteAdminAccess = Player.RemoteAdminAccess;
                writer = new NetDataWriter();
                writer.Put(NetworkData.RemoteAdminAccess);
                SendPacket(writer, Player, PlayerDataType.RemoteAdminAccess);
            }

            if (NetworkData.IsOverwatchEnabled != Player.IsOverwatchEnabled && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerOverwatch))
            {
                NetworkData.IsOverwatchEnabled = Player.IsOverwatchEnabled;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsOverwatchEnabled);
                SendPacket(writer, Player, PlayerDataType.Overwatch);
            }

            if (NetworkData.IPAddress != Player.IPAddress && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerIPAddress))
            {
                NetworkData.IPAddress = Player.IPAddress;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsOverwatchEnabled);
                SendPacket(writer, Player, PlayerDataType.IPAddress);
            }

            if (NetworkData.IsMuted != Player.IsMuted && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerMute))
            {
                NetworkData.IsMuted = Player.IsMuted;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsMuted);
                SendPacket(writer, Player, PlayerDataType.Mute);
            }

            if (NetworkData.IsIntercomMuted != Player.IsIntercomMuted)
            {
                NetworkData.IsIntercomMuted = Player.IsIntercomMuted;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsIntercomMuted);
                SendPacket(writer, Player, PlayerDataType.IntercomMute);
            }

            if (NetworkData.IsGodModeEnabled != Player.IsGodModeEnabled && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerGodmode))
            {
                NetworkData.IsGodModeEnabled = Player.IsGodModeEnabled;
                writer = new NetDataWriter();
                writer.Put(NetworkData.IsGodModeEnabled);
                SendPacket(writer, Player, PlayerDataType.Godmode);
            }

            if (NetworkData.Health != Player.Health && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerHealth))
            {
                NetworkData.Health = Player.Health;
                writer = new NetDataWriter();
                writer.Put(NetworkData.Health);
                SendPacket(writer, Player, PlayerDataType.Health);
            }

            if (NetworkData.MaxHealth != Player.MaxHealth && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerMaxHealth))
            {
                NetworkData.MaxHealth = Player.MaxHealth;
                writer = new NetDataWriter();
                writer.Put(NetworkData.MaxHealth);
                SendPacket(writer, Player, PlayerDataType.MaxHealth);
            }

            if (NetworkData.GroupName != Player.GroupName && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerGroupName))
            {
                NetworkData.GroupName = Player.GroupName;
                writer = new NetDataWriter();
                writer.Put(NetworkData.GroupName);
                SendPacket(writer, Player, PlayerDataType.GroupName);
            }

            if (NetworkData.RankColor != Player.RankColor && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRankColor))
            {
                NetworkData.RankColor = Player.RankColor;
                writer = new NetDataWriter();
                writer.Put(NetworkData.RankColor);
                SendPacket(writer, Player, PlayerDataType.RankColor);
            }

            if (NetworkData.RankName != Player.RankName && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerRankColor))
            {
                NetworkData.RankName = Player.RankName;
                writer = new NetDataWriter();
                writer.Put(NetworkData.RankName);
                SendPacket(writer, Player, PlayerDataType.RankName);
            }

            if (NetworkData.PlayerID != Player.Id && Extensions.CheckReceivePermission(AddonSendPermissionTypes.PlayerPlayerID))
            {
                NetworkData.PlayerID = Player.Id;
                writer = new NetDataWriter();
                writer.Put(NetworkData.PlayerID);
                SendPacket(writer, Player, PlayerDataType.PlayerID);
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
                    SendPacket(writer, Player, PlayerDataType.Position);
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
                    SendPacket(writer, Player, PlayerDataType.Rotation);
                }
            }
        }
    }
}
