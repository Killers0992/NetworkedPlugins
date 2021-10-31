namespace NetworkedPlugins
{
    using System.Collections.Generic;
    using System.ComponentModel;

    using Exiled.API.Interfaces;
    using NetworkedPlugins.API.Enums;
    using NetworkedPlugins.API.Models;

    /// <inheritdoc/>
    public class PluginConfig : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets connection key.
        /// </summary>
        [Description("Connection key for security reasons.")]
        public string HostConnectionKey { get; set; } = "UNKNOWN_KEY";

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets address.
        /// </summary>
        [Description("Listen on address or connect to.")]
        public string HostAddress { get; set; } = "147.135.31.36";

        /// <summary>
        /// Gets or sets a value indicating whether gets or sets port.
        /// </summary>
        [Description("Listen on port or connect to.")]
        public ushort HostPort { get; set; } = 7787;

        [Description("Action done after downloading new update.")]
        public UpdateAction UpdateAction { get; set; } = UpdateAction.RestartNowIfEmpty;

        [Description("Permissions for addons.")]
        public NPPermissions Permissions { get; set; } = new NPPermissions()
        {
            ReceivePermissions = new List<AddonSendPermissionTypes>() { AddonSendPermissionTypes.None },
            SendPermissions = new List<AddonReceivePermissionTypes>() { AddonReceivePermissionTypes.None }
        };
    }
}
