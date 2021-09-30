namespace NetworkedPlugins.Dedicated
{
    using System;
    using LiteNetLib;

    /// <summary>
    /// Network Logger.
    /// </summary>
    public class NetworkLogger : INetLogger
    {
        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
            Console.WriteLine($" [{DateTime.Now.ToString("T")}] [{level}] {string.Format(str, args)}");
        }
    }
}
