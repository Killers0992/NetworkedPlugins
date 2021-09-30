namespace NetworkedPlugins
{
    using Exiled.API.Features;

    /// <inheritdoc/>
    public class MainClass : Plugin<PluginConfig>
    {
        private NPClient client;
        private NPHost host;

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
        public override void OnEnabled()
        {
            if (Config.IsHost)
                host = new NPHost(this);
            else
                client = new NPClient(this);
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            if (client != null)
            {
                client.Unload();
                client = null;
            }

            if (host != null)
            {
                host.Unload();
                host = null;
            }
            base.OnDisabled();
        }
    }
}
