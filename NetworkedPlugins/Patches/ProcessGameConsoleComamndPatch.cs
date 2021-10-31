namespace NetworkedPlugins.Patches
{
	using CommandSystem;
	using HarmonyLib;
	using RemoteAdmin;
	using System;

	[HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.ProcessGameConsoleQuery))]
    internal class ProcessGameConsoleComamndPatch
    {
        public static bool Prefix(QueryProcessor __instance, string query)
        {
			string[] array = query.Trim().Split(QueryProcessor.SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);
			ICommand command;
			if (QueryProcessor.DotCommandHandler.TryGetCommand(array[0], out command))
			{
				try
				{
					string str;
					command.Execute(array.Segment(1), __instance._sender, out str);
					if (!string.IsNullOrEmpty(str))
						__instance.GCT.SendToClient(__instance.connectionToClient, array[0].ToUpperInvariant() + "#" + str, "");
				}
				catch (Exception arg)
				{
					__instance.GCT.SendToClient(__instance.connectionToClient, array[0].ToUpperInvariant() + "# Command execution failed! Error: " + arg, "");
				}
				return false;
			}
			__instance.GCT.SendToClient(__instance.connectionToClient, "Command not found.", "red");
			return false;
		}
    }
}
