namespace NetworkedPlugins.API.Enums
{
    public enum AddonReceivePermissionTypes : byte
    {
        None,
        Everything,
        ConsoleCommandExecution,
        RemoteAdminNewCommands,
        GameConsoleNewCommands,
        RemoteAdminMessages,
        GameConsoleMessages,
        HintMessages,
        ReportMessages,
        Broadcasts,
        ClearBroadcasts,
        Cassie,
        KillPlayer,
        RedirectPlayer,
        DisconnectPlayer,
        TeleportPlayer,
        GodmodePlayer,
        NoclipPlayer,
        ClearInventoryPlayer,
        Roundrestart
    }
}
