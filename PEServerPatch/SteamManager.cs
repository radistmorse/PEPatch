using Steamworks;
using UnityEngine;

namespace PEServerPatch
{
    public class SteamManager : MonoBehaviour
    {
        private static SteamManager instance;

        public static SteamManager Instance {
            get {
                if (instance == null)
                {
                    instance = new GameObject("SteamManager").AddComponent<SteamManager>();
                }

                return instance;
            }
        }

        private bool initialized;

        private Callback<SteamServersConnected_t> serverConnectedCallback;
        private Callback<SteamServerConnectFailure_t> serverNotConnectedCallback;
        private Callback<SteamServersDisconnected_t> serverDisconnectedCallback;

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);

            ushort port = (ushort)(uLink.Network.listenPort + 20);

            if (ServerConfig.PublicServer == false && port <= 9925)
            {
                port = (ushort)(port + 17095);
            }

            initialized = Steamworks.GameServer.Init(0u, 0, (ushort)uLink.Network.listenPort, port, EServerMode.eServerModeNoAuthentication, ServerConfig.ServerVersion.Replace("V",""));

            if (!initialized)
            {
                Debug.Log("[Steamworks.NET] GameServer_Init() failed. Verify your installation.");

                return;
            }

            serverConnectedCallback = Callback<SteamServersConnected_t>.CreateGameServer(ServerConnectedCallback);
            serverNotConnectedCallback = Callback<SteamServerConnectFailure_t>.CreateGameServer(ServerNotConnectedCallback);
            serverDisconnectedCallback = Callback<SteamServersDisconnected_t>.CreateGameServer(ServerDisconnectedCallback);
        }

        public void Update()
        {
            if (initialized)
            {
                Steamworks.GameServer.RunCallbacks();
            }
        }

        public void LogOff()
        {
            if (initialized && SteamGameServer.BLoggedOn())
            {
                SteamGameServer.EnableHeartbeats(false);
                SteamGameServer.LogOff();
            }
        }

        public void LogOn()
        {
            SteamGameServer.SetModDir("PlanetExplorers");
            SteamGameServer.SetProduct("PlanetExplorers");
            SteamGameServer.SetGameDescription("Planet Explorers");
            SteamGameServer.LogOnAnonymous();
        }

        public void SetAdditionalGameInfo()
        {
            var type = ServerConfig.GameType.ToString();
            var mode = ServerConfig.SceneMode.ToString();
            var UID = ServerConfig.ServerUID.ToString("X");
            var state = (uLinkNetwork.ServerStatus & CustomData.EServerStatus.Gameing) == CustomData.EServerStatus.Gameing ? 4 : 1;
            var numPlayers = uLink.Network.connections.Length;
            var masterName = ServerConfig.MasterRoleName;
            var pub = ServerConfig.PublicServer ? ",public" : "";
            SteamGameServer.SetGameTags($"{type},{mode},{UID},{state},{numPlayers},{masterName}{pub}");
        }

        private void ServerConnectedCallback(SteamServersConnected_t callback)
        {
            Debug.Log("[Steamworks.NET] GameServer connected successfully");

            SteamGameServer.SetServerName(ServerConfig.ServerName);
            SteamGameServer.SetPasswordProtected(!string.IsNullOrEmpty(ServerConfig.Password));
            SteamGameServer.SetMaxPlayerCount(ServerConfig.TeamNum * ServerConfig.NumPerTeam);
            SteamGameServer.SetMapName(ServerConfig.MapName);
            SteamGameServer.SetDedicatedServer(true);
            SetAdditionalGameInfo();
            SteamGameServer.EnableHeartbeats(true);
        }

        private void ServerNotConnectedCallback(SteamServerConnectFailure_t callback)
        {
            Debug.Log($"[Steamworks.NET] GameServer connection failed: {callback.m_eResult}");
        }

        private void ServerDisconnectedCallback(SteamServersDisconnected_t callback)
        {
            Debug.Log($"[Steamworks.NET] GameServer disconnected: {callback.m_eResult}");
        }
    }
}
