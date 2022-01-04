namespace NetworkedPlugins
{
    using Exiled.Events.EventArgs;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Packets.ClientPackets;

    public class EventHandlers
    {
        private NPClient Client;

        public EventHandlers(NPClient client) 
        {
            this.Client = client;
            Exiled.Events.Handlers.Server.LocalReporting += Server_LocalReporting;
            Exiled.Events.Handlers.Player.Verified += Player_Verified;
            Exiled.Events.Handlers.Player.Destroying += Player_Destroying;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.PreAuthenticating += Player_PreAuthenticating;
            Exiled.Events.Handlers.Server.RoundEnded += Server_RoundEnded;
        }

        public void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.LocalReporting -= Server_LocalReporting;
            Exiled.Events.Handlers.Player.Verified -= Player_Verified;
            Exiled.Events.Handlers.Player.Destroying -= Player_Destroying;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Server_WaitingForPlayers;
            Exiled.Events.Handlers.Player.PreAuthenticating -= Player_PreAuthenticating;
            Exiled.Events.Handlers.Server.RoundEnded -= Server_RoundEnded;
        }


        private void Player_Destroying(DestroyingEventArgs ev)
        {
            if (Client.Players.TryGetValue(ev.Player.UserId, out NetworkedPlayer plr))
            {
                UnityEngine.Object.Destroy(plr);
                Client.Players.Remove(ev.Player.UserId);
            }
        }


        private void Player_Verified(VerifiedEventArgs ev)
        {
            Client.Players.Add(ev.Player.UserId, ev.Player.GameObject.AddComponent<NetworkedPlayer>());
        }


        private void Server_WaitingForPlayers()
        {
            if (Client.NetworkListener == null)
                Client.StartNetworkClient();
            NPManager.Singleton.PacketProcessor.Send<EventPacket>(
                NPManager.Singleton.NetworkListener,
                new EventPacket()
                {
                    Type = (byte)EventType.WaitingForPlayers,
                    Data = new byte[0],
                },
                DeliveryMethod.ReliableOrdered);
        }


        private void Server_LocalReporting(LocalReportingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(ev.Issuer.UserId);
            writer.Put(ev.Target.UserId);
            writer.Put(ev.Reason);
            NPManager.Singleton.PacketProcessor.Send<EventPacket>(
                NPManager.Singleton.NetworkListener,
                new EventPacket()
                {
                    Type = (byte)EventType.PlayerLocalReport,
                    Data = writer.Data
                },
                DeliveryMethod.ReliableOrdered);
        }


        private void Player_PreAuthenticating(PreAuthenticatingEventArgs ev)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(ev.UserId);
            writer.Put(ev.Country);
            writer.Put(ev.Flags);
            NPManager.Singleton.PacketProcessor.Send<EventPacket>(
                NPManager.Singleton.NetworkListener,
                new EventPacket()
                {
                    Type = (byte)EventType.PreAuth,
                    Data = writer.Data
                },
                DeliveryMethod.ReliableOrdered);
        }

        private void Server_RoundEnded(RoundEndedEventArgs ev)
        {
            NPManager.Singleton.PacketProcessor.Send<EventPacket>(
                NPManager.Singleton.NetworkListener,
                new EventPacket()
                {
                    Type = (byte)EventType.RoundEnded,
                    Data = new byte[0],
                },
                DeliveryMethod.ReliableOrdered);
        }

    }
}
