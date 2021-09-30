namespace ExampleAddon
{
    using System;

    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Models;
    using LiteNetLib.Utils;

    /// <summary>
    /// Example host addon.
    /// </summary>
    public class ExampleAddonHost : NPAddonHost<AddonConfig>
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
            Logger.Info("Addon enabled on HOST.");
        }

        /// <inheritdoc/>
        public override void OnReady(NPServer server)
        {
            Logger.Info("Addon is ready");
        }

        /// <inheritdoc/>
        public override void OnMessageReceived(NPServer server, NetDataReader reader)
        {
            Logger.Info($"Received ( \"{reader.GetString()}\" ) from server {server.ServerAddress}:{server.ServerPort}");
            NetDataWriter writer = new NetDataWriter();
            writer.Put("Response");
            SendData(server.ServerAddress, server.ServerPort, writer);
        }

        /// <inheritdoc/>
        public override void OnConsoleResponse(NPServer server, string command, string response, bool isRa)
        {
            Logger.Info($"Received command response from server {server.FullAddress}, command name: {command}, response: {response}.");
        }
    }
}
