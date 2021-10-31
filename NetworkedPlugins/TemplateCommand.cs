namespace NetworkedPlugins
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using NetworkedPlugins.API;
    using NetworkedPlugins.API.Structs;
    using Exiled.Permissions.Extensions;
    using LiteNetLib;
    using System.Linq;
    using NetworkedPlugins.API.Enums;

    /// <summary>
    /// Template command.
    /// </summary>
    public class TemplateCommand : ICommand
    {
        /// <summary>
        /// Gets or sets command name.
        /// </summary>
        public string DummyCommand { get; set; } = string.Empty;

        /// <summary>
        /// Gets command name.
        /// </summary>
        public string Command => DummyCommand;

        /// <summary>
        /// Gets command aliases.
        /// </summary>
        public string[] Aliases => new string[0];

        /// <summary>
        /// Gets or sets command description.
        /// </summary>
        public string DummyDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets command description.
        /// </summary>
        public string Description => DummyDescription;

        /// <summary>
        /// Gets or sets command permission.
        /// </summary>
        public string Permission { get; set; }

        /// <summary>
        /// Gets or sets assigned addon id.
        /// </summary>
        public string AssignedAddonID { get; set; }
        
        public byte Type { get; set; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player p = Player.Get(sender);
            if (p == null)
            {
                response = "Player not found";
                return false;
            }

            if (string.IsNullOrEmpty(Permission))
                goto skipPermCheck;

            if (!p.CheckPermission(Permission))
            {
                response = $"Missing required permission \"{Permission}\".";
                return false;
            }

            skipPermCheck:
            NPManager.Singleton.PacketProcessor.Send<ExecuteCommandPacket>(NPManager.Singleton.NetworkListener, 
                new ExecuteCommandPacket() 
                { 
                    UserID = p.UserId, 
                    AddonID = AssignedAddonID,
                    Type = Type, 
                    CommandName = this.Command, 
                    Arguments = arguments.ToArray() 
                }, DeliveryMethod.ReliableOrdered);
            response = string.Empty;
            return false;
        }
    }
}
