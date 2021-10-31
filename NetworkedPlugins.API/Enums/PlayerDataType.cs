using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkedPlugins.API.Enums
{
    public enum PlayerDataType : byte
    {
        Nickname,
        Role,
        DoNotTrack,
        RemoteAdminAccess,
        Overwatch,
        IPAddress,
        Mute,
        IntercomMute,
        Godmode,
        Health,
        MaxHealth,
        GroupName,
        RankColor,
        RankName,
        PlayerID,
        Position,
        Rotation
    }
}
