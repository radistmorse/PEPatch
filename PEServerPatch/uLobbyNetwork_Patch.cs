using HarmonyLib;
using Steamworks;

namespace PEServerPatch
{
    [HarmonyPatch(typeof(uLobbyNetwork))]
    class uLobbyNetwork_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("uLink_OnServerInitialized")]
        public static bool uLink_OnServerInitialized()
        {
            SteamManager.Instance.LogOn();
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateServerInfo")]
        public static bool UpdateServerInfo()
        {
            if (SteamGameServer.BLoggedOn())
            {
                SteamManager.Instance.SetAdditionalGameInfo();
            }

            return false;
        }
    }
}
