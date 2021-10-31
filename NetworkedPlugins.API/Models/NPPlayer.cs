namespace NetworkedPlugins.API.Structs
{
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Structs;

    using LiteNetLib;
    using LiteNetLib.Utils;
    using System.Reflection;
    using NetworkedPlugins.API.Extensions;
    using NetworkedPlugins.API.Enums;

    /// <inheritdoc/>
    public class NPPlayer : PlayerFuncs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NPPlayer"/> class.
        /// </summary>
        /// <param name="server">Server where player is on.</param>
        /// <param name="userID">Player UserID.</param>
        public NPPlayer(NPServer server, string userID)
        {
            this.UserID = userID;
            this.Server = server;
        }

        /// <summary>
        /// Gets or sets erver where player is online.
        /// </summary>
        public NPServer Server { get; internal set; }

        /// <inheritdoc/>
        public override void Kill()
        {
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.KillPlayer,
                Data = new byte[0],
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendReportMessage(string message)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.ReportMessage,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendRAMessage(string message, string pluginName = "NP")
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(pluginName);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.RemoteAdminMessage,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendConsoleMessage(string message, string pluginName = "NP", string color = "GREEN")
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(pluginName);
            writer.Put(color);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.GameConsoleMessage,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void Redirect(ushort port)
        {
            var writer = new NetDataWriter();
            writer.Put(port);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.Redirect,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void Disconnect(string reason)
        {
            var writer = new NetDataWriter();
            writer.Put(reason);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.Disconnect,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendHint(string message, float duration)
        {
            var writer = new NetDataWriter();
            writer.Put(message);
            writer.Put(duration);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.Hint,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendPosition(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.SendPosition,
                Data = writer.Data,
            }, DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SendRotation(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.SendRotation,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void Teleport(float x, float y, float z)
        {
            var writer = new NetDataWriter();
            writer.Put(x);
            writer.Put(y);
            writer.Put(z);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.Teleport,
                Data = writer.Data
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SetGodmode(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket()
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.Godmode,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void SetNoclip(bool state = false)
        {
            var writer = new NetDataWriter();
            writer.Put(state);
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID,
                Type = (byte)PlayerInteractionType.Noclip,
                Data = writer.Data 
            }, LiteNetLib.DeliveryMethod.ReliableOrdered);
        }

        /// <inheritdoc/>
        public override void ClearInventory()
        {
            Server.PacketProcessor.Send<PlayerInteractPacket>(Server.Peer, new PlayerInteractPacket() 
            {
                AddonId = Assembly.GetExecutingAssembly().GetAddonId(),
                UserID = UserID, 
                Type = (byte)PlayerInteractionType.ClearInventory,
                Data = new byte[0], 
            }, DeliveryMethod.ReliableOrdered);
        }
    }
}
