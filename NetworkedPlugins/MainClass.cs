namespace NetworkedPlugins
{
    using Exiled.API.Features;
    using HarmonyLib;
    using NetworkedPlugins.API;
    using System;

    /// <inheritdoc/>
    public class MainClass : Plugin<PluginConfig>
    {
        private NPClient client;
        private Harmony harmony;

        /// <summary>
        /// Gets or Sets singleton of main plugin class.
        /// </summary>
        public static MainClass Singleton { get; set; }

        /// <inheritdoc/>
        public override string Name { get; } = "NetworkedPlugins";

        /// <inheritdoc/>
        public override string Prefix { get; } = "networkedplugins";

        /// <inheritdoc/>
        public override string Author { get; } = "Killers0992";

        /// <inheritdoc/>
        public override Version Version => NPVersion.Version;

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new Version(4, 0, 0);

        private string LastId;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            harmony = new Harmony($"networkedplugins.{DateTime.Now.Ticks}");
            harmony.PatchAll();

            Singleton = this;
            client = new NPClient(this);
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            harmony.UnpatchAll(harmony.Id);
            harmony = null;

            Singleton = null;
            if (client != null)
            {
                client.Unload();
                client = null;
            }

            base.OnDisabled();
        }
    }
}
