using HarmonyLib;

namespace PEServerPatch
{
    [HarmonyPatch(typeof(uLinkNetwork))]
    public class uLinkNetwork_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch("uLink_OnServerUninitialized")]
        public static void uLink_OnServerUninitialized()
        {
            SteamManager.Instance.LogOff();
        }
    }
}
