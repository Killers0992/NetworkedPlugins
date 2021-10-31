namespace ExampleAddon
{
    using System;
    using System.Collections.Generic;

    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Models;

    using LiteNetLib.Utils;

    /// <summary>
    /// Example dedicated addon.
    /// </summary>
    public class ExampleAddonDedicated : NPAddonDedicated<AddonConfig, AddonConfig>
    {
        /// <inheritdoc/>
        public override string AddonAuthor { get; } = "Killers0992";

        /// <inheritdoc/>
        public override string AddonName { get; } = "ExampleAddon";

        /// <inheritdoc/>
        public override Version AddonVersion { get; } = new Version(1, 0, 0);

        /// <inheritdoc/>
        public override string AddonId { get; } = "0dewadopsdap32";

        /// <inheritdoc/>
        public override void OnEnable()
        {
            Logger.Info("Addon enabled on DEDICATED HOST.");
        }

        /// <inheritdoc/>
        public override void OnMessageReceived(NetDataReader reader)
        {
            Logger.Info($"Received message from server {Server.ServerAddress}:{Server.ServerPort}");
            foreach (var plr in Server.Players)
            {
                Logger.Info($"Player {plr.Nickname} {plr.UserID}");
            }

            NetDataWriter writer = new NetDataWriter();
            writer.Put("Response");
            SendData(writer);
        }

        /// <inheritdoc/>
        public override void OnConsoleCommand(string cmd, List<string> arguments)
        {
            switch (cmd.ToUpper())
            {
                case "TEST":
                    Logger.Info("Test response");
                    break;
            }
        }
    }
}
