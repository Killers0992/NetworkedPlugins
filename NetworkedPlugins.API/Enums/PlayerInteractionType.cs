namespace NetworkedPlugins.API.Enums
{
    public enum PlayerInteractionType : byte
    {
        KillPlayer,
        ReportMessage,
        RemoteAdminMessage,
        GameConsoleMessage,
        Redirect,
        Disconnect,
        Hint,
        SendPosition,
        SendRotation,
        Teleport,
        Godmode,
        Noclip,
        ClearInventory
    }
}
