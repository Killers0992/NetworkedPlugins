namespace NetworkedPlugins.API.Packets
{
    public class AddonDataPacket
    {
        public string AddonId { get; set; }
        public byte[] Data { get; set; }
    }
}
