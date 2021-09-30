namespace ExampleAddon
{
    using NetworkedPlugins.API.Interfaces;

    /// <summary>
    /// Config for example addon.
    /// </summary>
    public class AddonConfig : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;
    }
}
