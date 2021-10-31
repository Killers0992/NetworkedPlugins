namespace ExampleAddon
{
    using System;
    using System.Collections.Generic;
    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Models;
    using LiteNetLib.Utils;
    using MEC;

    /// <summary>
    /// Example client addon.
    /// </summary>
    public class ExampleAddonClient : NPAddonClient<AddonConfig>
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
            Logger.Info("Addon enabled on CLIENT.");
            Timing.RunCoroutine(SendDatas());
        }

        /// <inheritdoc/>
        public override void OnReady()
        {
            Logger.Info("Addon is ready");
        }

        /// <inheritdoc/>
        public override void OnMessageReceived(NetDataReader reader)
        {
            Logger.Info($"Received ( \"{reader.GetString()}\" )");
        }

        private IEnumerator<float> SendDatas()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(3f);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Some string");
                SendData(writer);
            }
        }
    }
}
