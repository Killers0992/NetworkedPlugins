using NetworkedPlugins.API.Enums;
using System.Collections.Generic;

namespace NetworkedPlugins.API.Models
{
    public class NPPermissions
    {
        public List<AddonReceivePermissionTypes> SendPermissions { get; set; }
        public List<AddonSendPermissionTypes> ReceivePermissions { get; set; }
    }
}
