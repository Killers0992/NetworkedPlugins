namespace NetworkedPlugins.API.Interfaces
{
    using NetworkedPlugins.API.Structs;

    /// <summary>
    /// Player functions.
    /// </summary>
    public abstract class PlayerFuncs : NetworkedPlayerData
    {
        /// <summary>
        /// Kill player.
        /// </summary>
        public abstract void Kill();

        /// <summary>
        /// Send report message to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void SendReportMessage(string message);

        /// <summary>
        /// Send remoteadmin message to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        public abstract void SendRAMessage(string message, string pluginName = "");

        /// <summary>
        /// Send console message to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="color">Message color.</param>
        public abstract void SendConsoleMessage(string message, string pluginName = "", string color = "GREEN");

        /// <summary>
        /// Redirect player to other server.
        /// </summary>
        /// <param name="port">Server port.</param>
        public abstract void Redirect(ushort port);

        /// <summary>
        /// Disconnect player from server.
        /// </summary>
        /// <param name="reason">Reason of disconnect.</param>
        public abstract void Disconnect(string reason);

        /// <summary>
        /// Send hint to player.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="duration">The duration of hint.</param>
        public abstract void SendHint(string message, float duration);

        /// <summary>
        /// If player should send position data.
        /// </summary>
        /// <param name="state">State set to true means data about position will be sended via network.</param>
        public abstract void SendPosition(bool state = false);

        /// <summary>
        /// If player should send rotation data.
        /// </summary>
        /// <param name="state">State set to true means data about rotation will be sended via network.</param>
        public abstract void SendRotation(bool state = false);

        /// <summary>
        /// Teleport player.
        /// </summary>
        /// <param name="x">X Cordinate.</param>
        /// <param name="y">Y Cordinate.</param>
        /// <param name="z">Z Cordinate.</param>
        public abstract void Teleport(float x, float y, float z);

        /// <summary>
        /// Set ghostmode.
        /// </summary>
        /// <param name="state">State of godmode.</param>
        public abstract void SetGodmode(bool state = false);

        /// <summary>
        /// Set noclip.
        /// </summary>
        /// <param name="state">State of noclip.</param>
        public abstract void SetNoclip(bool state = false);

        /// <summary>
        /// Clear player inventory.
        /// </summary>
        public abstract void ClearInventory();
    }
}
