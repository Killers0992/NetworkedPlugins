namespace NetworkedPlugins.API.Models
{
    using LiteNetLib.Utils;

    /// <summary>
    /// Player info packet.
    /// </summary>
    public struct PlayerInfo : INetSerializable
    {
        /// <summary>
        /// Gets userid.
        /// </summary>
        public string UserID;

        /// <summary>
        /// Deserialize.
        /// </summary>
        /// <param name="reader">Data reader.</param>
        public void Deserialize(NetDataReader reader)
        {
            UserID = reader.GetString();
        }

        /// <summary>
        /// Serializer.
        /// </summary>
        /// <param name="writer">Data writer.</param>
        public void Serialize(NetDataWriter writer)
        {
            writer.Put(UserID);
        }
    }
}
