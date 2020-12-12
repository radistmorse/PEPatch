# PEPatch
Multiplayer patch for Planet Explorers. Works for Windows x64.

# To start the server
- Download the PEServerPatch zipfile from the latest release.
- Unpack it to the ```Server``` folder inside your PE installation.
- Configure the server using the file ```ConfigFiles/ServerConfig.conf```.
  - Set ```ServerName``` to something distinguishable.
  - Set ``PublicServer``` to true if you want it to be accessible through internet, or leave it false to make it a LAN-server.
  - Set the password if needed.
  - Configure the rest of the config normally (see the explanation e.g. [here](https://steamcommunity.com/app/237870/discussions/2/152390014783676975/)).
- If you chose the public server, configure the router to forward the ports 9900-9940 (you need 9900 and 9920 for the first instance, 9901 and 9921 for the second and so on).
  - if you chose the LAN server, another port range will be used (9900-9920 and 27015-27020), because that's what steam expects. No need to forward these ports, since it only works in LAN anyway.
- Start the server by launching the ```PE_Server.exe```.
  - **IMPORTANT**: do not start the server through the in-game menu. I have no idea how this works and how my patch would affect that.
- If everything went correctly, you should see the file ```output_log.txt``` created in the folder. Look inside it for the line
  ```
  [Steamworks.NET] GameServer connected successfully
  ```
  If you see it, the server started successfully.

# To join the server
- Download the PEPatch zipfile from the latest release.
- Unpack it to the root folder of the PE installation.
- Launch, click the ```Multiplayer``` button, create your character.
- In the list of servers you should see your server either in ```Internet``` tab (if the ```PublicServer``` was set to ```true```) or in ```LAN```.
  - **IMPORTANT**: if ```PublicServer``` was set to ```true```, you won't see your server in the ```LAN``` tab even if it is in your LAN. Setting the ```PublicServer``` to ```false``` is the only way to connect to the server without internet access.
- You could provide a custom characterid/steamid as a comma-separated line in the "Player filter" input. You probably don't need it, if you don't know what it is.
- The game can crash randomly during the lobby update. This is expected. I can't fix it, since the game was not supposed to integrate with steam in such a way. Just restart the game. As soon as you connect to the server this shouldn't affect you anymore.
- In case of a weird behaviour look inside the ```output_log.txt``` in the root folder of PE. Search for lines with the words ```Exception``` or ```[STEAMWORKS]``` to get a clue of what's wrong.
