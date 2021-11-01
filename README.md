# NetworkedPlugins
Connect SL servers and send data between them.


# Setup

1. Download [Plugin](github.com/Killers0992/NetworkedPlugins/releases/latest/download/NetworkedPlugins.dll) and needed [dependency](github.com/Killers0992/NetworkedPlugins/releases/latest/download/NetworkedPlugins.API.dll).

2. Move ``NetworkedPlugins.dll`` to ``Exiled/Plugins`` and ``NetworkedPlugins.API.dll`` to ``Exiled/Plugins/dependencies``.

3. Start server and wait when plugin generates ``Exiled/Plugins/NetworkedPlugins/addons-<serverPort>``.

4. Put [example addon](github.com/Killers0992/NetworkedPlugins/releases/latest/download/ExampleAddon.dll) in ``Exiled/Plugins/NetworkedPlugins/addons-<serverPort>``.

5. How host?
- If you have way hosting dedicated app use [this](https://github.com/Killers0992/NetworkedPlugins/tree/master#using-dedicated-app-to-host).

6. How connect?

- You need to use same ``host_connection_key`` from DedicatedApp to properly connect to that server,
also you need to have same ``host_port``.

# USING Dedicated APP to HOST

- Take [Windows](github.com/Killers0992/NetworkedPlugins/releases/latest/download/DedicatedApp-Windows.zip)/[Linux](github.com/Killers0992/NetworkedPlugins/releases/latest/download/DedicatedApp-Linux.zip) ( depends on your server OS )
- Move all files from downloaded zip to your server and run 
- ``./NetworkedPlugins.Dedicated`` ( LINUX )
- ``./NetworkedPlugins.Dedicated.exe`` ( WINDOWS ) 

# Some info:

If your host server and server which connects is on localhost you dont need to change ``host_address`` but if its on other machine then you need to set that value to public ip.

``host_connection_key`` - Its just password which its needed while connecting to other servers.

``host_port`` - UDP port which will be used for hosting or connecting to server.


# Example Addon
[LINK](https://github.com/Killers0992/NetworkedPlugins/tree/master/ExampleAddon)

Addon classes are seperated soo addons can run without SL Server and with,
if you will use SL/Exiled methods in class which inherits NPAddonDedicated then that will not work, you need to inherit NPAddonClient or NPAddonDedicated


Which things can be done with that?

- Global broadcasts which are sended to all servers.
- Report system which works between servers and admins can easily use command to connect to that server.
- Commands which are created from host/dedicated app and clients ( server which have IsHost set to false ) can receive them without needing to restart SL Servers.
- Addons can work without installing them on other server ( can be only on host / dedicated app ) but then limitation is from API side of plugin.

For example class which inherits NPAddon(Client/Dedicated/Host) can use method GetServers() which contains all connected servers, every server have methods for example stuff like broadcasts/restarts or other stuff also its way of getting all players and doing stuff for them, changing noclip/godmode check health, kill or redirect them to other servers
