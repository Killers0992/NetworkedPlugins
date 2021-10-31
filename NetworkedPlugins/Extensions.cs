using NetworkedPlugins.API.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins
{
    public static class Extensions
    {
        public static bool CheckSendPermission(AddonReceivePermissionTypes type)
        {
            if (MainClass.Singleton.Config.Permissions.SendPermissions.Contains(AddonReceivePermissionTypes.Everything))
                return true;

            return MainClass.Singleton.Config.Permissions.SendPermissions.Contains(type);
        }

        public static bool CheckReceivePermission(AddonSendPermissionTypes type)
        {
            if (MainClass.Singleton.Config.Permissions.ReceivePermissions.Contains(AddonSendPermissionTypes.Everything))
                return true;

            return MainClass.Singleton.Config.Permissions.ReceivePermissions.Contains(type);
        }

    }
}
