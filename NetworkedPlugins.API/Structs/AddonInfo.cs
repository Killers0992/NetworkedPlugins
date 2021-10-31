using LiteNetLib.Utils;
using NetworkedPlugins.API.Enums;
using System.Collections.Generic;

namespace NetworkedPlugins.API.Packets
{
    public struct AddonInfo : INetSerializable
    {
        public string AddonId { get; set; }
        public string AddonName { get; set; }
        public string AddonAuthor { get; set; }
        public string AddonVersion { get; set; }
        public byte[] SendPermissions { get; set; }
        public byte[] ReceivePermissions { get; set; }
        public byte[] RemoteConfig { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            AddonId = reader.GetString();
            AddonName = reader.GetString();
            AddonAuthor = reader.GetString();
            AddonVersion = reader.GetString();

            SendPermissions = reader.GetBytesWithLength();
            ReceivePermissions = reader.GetBytesWithLength();

            RemoteConfig = reader.GetBytesWithLength();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(AddonId);
            writer.Put(AddonName);
            writer.Put(AddonAuthor);
            writer.Put(AddonVersion);

            writer.PutBytesWithLength(SendPermissions);
            writer.PutBytesWithLength(ReceivePermissions);

            writer.PutBytesWithLength(RemoteConfig, 0, RemoteConfig.Length);
        }
    }
}
