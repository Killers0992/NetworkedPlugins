namespace NetworkedPlugins.Dedicated
{
    using System.ComponentModel;

    /// <summary>
    /// Network config.
    /// </summary>
    public class DedicatedConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether gets or sets connection key.
        /// </summary>
        [Description("Connection key for security reasons.")]
        public string HostConnectionKey { get; set; } = "UNKNOWN_KEY";

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets address.
        /// </summary>
        [Description("Listen on address or connect to.")]
        public string HostAddress { get; set; } = "localhost";

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets port.
        /// </summary>
        [Description("Listen on port or connect to.")]
        public ushort HostPort { get; set; } = 7777;
    }
}
