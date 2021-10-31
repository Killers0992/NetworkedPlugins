using NetworkedPlugins.API.Enums;

namespace NetworkedPlugins.API.Structs
{
    public class NetworkedPlayerData
    {
        public bool IsConnected { get; set; }

        /// <summary>
        /// Gets or sets the player's user id.
        /// </summary>
        public virtual string UserID { get; internal set; }

        /// <summary>
        /// Gets or sets the player's nickname.
        /// </summary>
        public virtual string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the player's <see cref="int"/>.
        /// </summary>
        public virtual PlayerRole Role { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can be tracked.
        /// </summary>
        public virtual bool DoNotTrack { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has Remote Admin access.
        /// </summary>
        public virtual bool RemoteAdminAccess { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player's overwatch is enabled.
        /// </summary>
        public virtual bool IsOverwatchEnabled { get; set; }

        /// <summary>
        /// Gets or sets the player's IP address.
        /// </summary>
        public virtual string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is muted.
        /// </summary>
        public virtual bool IsMuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player is intercom muted.
        /// </summary>
        public virtual bool IsIntercomMuted { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player has godmode enabled.
        /// </summary>
        public virtual bool IsGodModeEnabled { get; set; }

        /// <summary>
        /// Gets or sets the player's health.
        /// If the health is greater than the <see cref="MaxHealth"/>, the MaxHealth will also be changed to match the health.
        /// </summary>
        public virtual float Health { get; set; }

        /// <summary>
        /// Gets or sets the player's maximum health.
        /// </summary>
        public virtual int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the player's group name.
        /// </summary>
        public virtual string GroupName { get; set; }

        /// <summary>
        /// Gets or sets the player's rank color.
        /// </summary>
        public virtual string RankColor { get; set; }

        /// <summary>
        /// Gets or sets the player's rank name.
        /// </summary>
        public virtual string RankName { get; set; }

        /// <summary>
        /// Gets or sets the player's id.
        /// </summary>
        public virtual int PlayerID { get; set; }

        /// <summary>
        /// Gets or sets the player's position.
        /// </summary>
        public virtual Position Position { get; set; }

        /// <summary>
        /// Gets or sets the player's rotation.
        /// </summary>
        public virtual Rotation Rotation { get; set; }

    }
}
