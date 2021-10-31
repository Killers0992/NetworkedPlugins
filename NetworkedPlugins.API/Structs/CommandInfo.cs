namespace NetworkedPlugins.API.Models
{
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Enums;
    using System;

    /// <summary>
    /// Command info.
    /// </summary>
    public struct CommandInfo : INetSerializable
    {
        /// <summary>
        /// Gets addon id.
        /// </summary>
        public string AddonID;

        /// <summary>
        /// Gets command name.
        /// </summary>
        public string CommandName;

        /// <summary>
        /// Gets command description.
        /// </summary>
        public string Description;

        /// <summary>
        /// Gets command permission.
        /// </summary>
        public string Permission;

        /// <summary>
        /// Gets command type.
        /// </summary>
        public byte Type;

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="reader">Data reader.</param>
        public void Deserialize(NetDataReader reader)
        {
            AddonID = reader.GetString();
            CommandName = reader.GetString();
            Description = reader.GetString();
            Permission = reader.GetString();
            Type = reader.GetByte();
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(AddonID);
            writer.Put(CommandName);
            writer.Put(Description);
            writer.Put(Permission);
            writer.Put(Type);
        }
    }
}
