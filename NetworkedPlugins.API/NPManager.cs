namespace NetworkedPlugins.API
{
    using System.Collections.Generic;
    using System.IO;
    using LiteNetLib;
    using LiteNetLib.Utils;
    using NetworkedPlugins.API.Interfaces;
    using NetworkedPlugins.API.Structs;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using NetworkedPlugins.API.Extensions;
    using System.Reflection;

    /// <summary>
    /// Network Manager.
    /// </summary>
    public class NPManager
    {
        /// <summary>
        /// Gets or Sets singleton of npmanager.
        /// </summary>
        public static NPManager Singleton { get; set; }

        /// <summary>
        /// Gets the serializer for configs.
        /// </summary>
        public static ISerializer Serializer { get; } = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();

        /// <summary>
        /// Gets the deserializer for configs.
        /// </summary>
        public static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        /// <summary>
        /// Gets or sets Packet Processor.
        /// </summary>
        public NetPacketProcessor PacketProcessor { get; set; } = new NetPacketProcessor();

        /// <summary>
        /// Gets or sets Logger.
        /// </summary>
        public NPLogger Logger { get; set; }

        /// <summary>
        /// Gets or sets Network Listener.
        /// </summary>
        public NetManager NetworkListener { get; set; }

        /// <summary>
        /// Gets or sets dictionary of all loaded client addons.
        /// </summary>
        public Dictionary<string, IAddonClient<IConfig>> ClientAddons { get; } = new Dictionary<string, IAddonClient<IConfig>>();

        /// <summary>
        /// Gets or sets dictionary of all loaded dedicated addons handlers.
        /// </summary>
        public Dictionary<string, IAddonHandler<IConfig>> DedicatedAddonHandlers { get; } = new Dictionary<string, IAddonHandler<IConfig>>();
        
        /// <summary>
        /// Gets or sets dictionary of all online servers.
        /// </summary>
        public Dictionary<NetPeer, NPServer> Servers { get; } = new Dictionary<NetPeer, NPServer>();

        public Dictionary<Assembly, string> AddonAssemblies { get; } = new Dictionary<Assembly, string>();

        /// <summary>
        /// Register command from addon.
        /// </summary>           
        /// <param name="addon">Addon.</param>
        /// <param name="command">Command interface.</param>
        public void RegisterCommand(IAddonDedicated<IConfig, IConfig> addon, ICommand command)
        {
            if (addon.Commands[command.Type].ContainsKey(command.CommandName.ToUpper()))
            {
                Logger.Info($"[{command.Type}] Command \"{command.CommandName.ToUpper()}\" is already registered in addon \"{addon.AddonName}\"!");
                return;
            }

            addon.Commands[command.Type].Add(command.CommandName.ToUpper(), command);
            Logger.Info($"[{command.Type}] Command \"{command.CommandName.ToUpper()}\" registered in addon \"{addon.AddonName}\".");
        }

        /// <summary>
        /// Load addon config.
        /// </summary>
        /// <param name="addon">Addon.</param>
        public void LoadAddonConfig(IAddonClient<IConfig> addon)
        {
            if (!Directory.Exists(addon.AddonPath))
                Directory.CreateDirectory(addon.AddonPath);

            if (!File.Exists(Path.Combine(addon.AddonPath, "config.yml")))
                File.WriteAllText(Path.Combine(addon.AddonPath, "config.yml"), Serializer.Serialize(addon.Config));

            var cfg = (IConfig)Deserializer.Deserialize(File.ReadAllText(Path.Combine(addon.AddonPath, "config.yml")), addon.Config.GetType());
            File.WriteAllText(Path.Combine(addon.AddonPath, "config.yml"), Serializer.Serialize(cfg));
            addon.Config.CopyProperties(cfg);
        }


        /// <summary>
        /// Load addon config.
        /// </summary>
        /// <param name="addon">Addon.</param>
        public void LoadAddonConfig(IAddonDedicated<IConfig, IConfig> addon)
        {
            if (!Directory.Exists(addon.AddonPath))
                Directory.CreateDirectory(addon.AddonPath);

            if (!File.Exists(Path.Combine(addon.AddonPath, "config.yml")))
                File.WriteAllText(Path.Combine(addon.AddonPath, "config.yml"), Serializer.Serialize(addon.Config));

            var cfg = (IConfig)Deserializer.Deserialize(File.ReadAllText(Path.Combine(addon.AddonPath, "config.yml")), addon.Config.GetType());
            File.WriteAllText(Path.Combine(addon.AddonPath, "config.yml"), Serializer.Serialize(cfg));
            addon.Config.CopyProperties(cfg);
        }
    }
}
