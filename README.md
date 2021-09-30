# NetworkedPlugins
Connect SL servers and send data between them.


# Setup

1. Unzip ``NetworkedPlugins.zip``

2. Everything from Exiled Plugin dictionary move to ``Exiled/Plugins``.

3. Start server and wait when plugin generates ``Exiled/Plugins/NetworkedPlugins/addons-<serverPort>``.

4. Put example addon from ``Example Addon`` dictionary in ``Exiled/Plugins/NetworkedPlugins/addons-<serverPort>``.

5. How host?
- If you have more than 1 SL server you can use [that](https://github.com/Killers0992/NetworkedPlugins/tree/master#using-sl-server-to-host).
- If you have way hosting dedicated app use [this](https://github.com/Killers0992/NetworkedPlugins/tree/master#using-dedicated-app-to-host).

# USING SL SERVER TO HOST

- Server must have in config IsHost set to true
( That means that server will be handling everything )

# USING Dedicated APP to HOST

- In Dedicated App dictionary take ``Windows/Linux`` ( depends on your server OS )
- Move all files from ``Dedicated App/(Windows/Linux)`` to your server and run 
- ``./NetworkedPlugins.Dedicated`` ( LINUX )
- ``./NetworkedPlugins.Dedicated.exe`` ( WINDOWS ) 

6. How connect?

- ``IsHost`` this time is set to ``false`` and you need to use same ``host_connection_key`` from Host server or DedicatedApp to properly connect to that server,
also you need to have same ``host_port``.

If your host server and server which connects is on localhost you dont need to change ``host_address`` but if its on other machine then you need to set that value to public ip.


# Some info:

``host_connection_key`` - Its just password which its needed while connecting to other servers.

``host_port`` - UDP port which will be used for hosting or connecting to server.


# Example Addon
[LINK](https://github.com/Killers0992/EXILED/tree/dev/Exiled.NetworkExample)

Addon classes are seperated soo addons can run without SL Server and with,
if you will use SL/Exiled methods in class which inherits NPAddonDedicated then that will not work, you need to inherit NPAddonClient or NPAddonHost


Which things can be done with that?

- Global broadcasts which are sended to all servers.
- Report system which works between servers and admins can easily use command to connect to that server.
- Commands which are created from host/dedicated app and clients ( server which have IsHost set to false ) can receive them without needing to restart SL Servers.
- Addons can work without installing them on other server ( can be only on host / dedicated app ) but then limitation is from API side of plugin.

For example class which inherits NPAddon(Client/Dedicated/Host) can use method GetServers() which contains all connected servers, every server have methods for example stuff like broadcasts/restarts or other stuff also its way of getting all players and doing stuff for them, changing noclip/godmode check health, kill or redirect them to other servers
