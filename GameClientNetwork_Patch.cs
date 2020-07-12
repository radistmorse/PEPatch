using HarmonyLib;

namespace PEPatch
{
    [HarmonyPatch(typeof(GameClientNetwork))]
    public class GameClientNetwork_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("uLink_OnFailedToConnect")]
        public static bool uLink_OnFailedToConnect(uLink.NetworkConnectionError error)
        {
            if (error == uLink.NetworkConnectionError.InvalidPassword && UILobbyMainWndCtrl.Instance != null)
            {
                MessageBox_N.CancelMask(MsgInfoType.ServerLoginMask);

                var serverData = Traverse.Create(UILobbyMainWndCtrl.Instance).Field("mSelectServerData").GetValue();

                if (serverData is ServerRegistered)
                {
                    Traverse.Create(serverData).Field("PasswordStatus").SetValue(1);
                    UILobbyMainWndCtrl.Instance.mCheckPasswordInput.text = string.Empty;
                    UILobbyMainWndCtrl.Instance.mPassWordWnd.SetActive(true);
                    return false;
                }
            }

            return true;
        }

    }
}
