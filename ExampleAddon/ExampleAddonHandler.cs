using NetworkedPlugins.API;
using NetworkedPlugins.API.Events.Player;

namespace ExampleAddon
{
    public class ExampleAddonHandler : NPAddonHandler<AddonConfig>
    {
        public override void OnEnable()
        {
            this.PlayerJoined += OnPlayerJoined;
            base.OnEnable();
        }

        public override void OnDisable()
        {
            this.PlayerJoined -= OnPlayerJoined;
            base.OnDisable();
        }

        private void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            Logger.Info($"Player {ev.Player.UserID} joined server {ev.Player.Server.FullAddress}.");
        }
    }
}
