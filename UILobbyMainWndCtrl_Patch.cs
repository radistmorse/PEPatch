using HarmonyLib;
using System.Collections;

namespace PEPatch
{
    [HarmonyPatch(typeof(UILobbyMainWndCtrl))]
    class UILobbyMainWndCtrl_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Start")]
        public static bool Start(UILobbyMainWndCtrl __instance)
        {
            PeSteamFriendMgr.Instance.Init(__instance.mTopLeftAuthor.transform, __instance.mUICenter.transform, __instance.mUICamera);

            if (GameClientLobby.Self == null)
                return false;

            Traverse.Create(__instance).Field("mRoleInfo").SetValue(GameClientLobby.role);

            __instance.GetType().GetMethod("SetRoleInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { });

            var mRecentRoom_M = new RecentRoomDataManager(Traverse.Create(__instance).Field("mRoleInfo").Field("name").GetValue<string>());

            Traverse.Create(__instance).Field("mRecentRoom_M").SetValue(mRecentRoom_M);
            mRecentRoom_M.LoadFromFile();

            __instance.StartCoroutine((IEnumerator)__instance.GetType().GetMethod("UpdatePlayerInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { }));
            __instance.StartCoroutine((IEnumerator)__instance.GetType().GetMethod("UpdateRoomInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { }));

            __instance.GetType().GetMethod("InitRoomListSort", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { });

            __instance.mRoomInput.bUseClipboard = true;

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool Update(UILobbyMainWndCtrl __instance)
        {
            __instance.GetType().GetMethod("UpdateLobbyLevel", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { });

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("WorkShopOnClose")]
        public static bool WorkShopOnClose(UILobbyMainWndCtrl __instance)
        {
            var mWorkShopCtrl = Traverse.Create(__instance).Field("mWorkShopCtrl").GetValue<UIWorkShopCtrl>();
            if (mWorkShopCtrl != null)
            {
                var mLastWnd = Traverse.Create(__instance).Field("mLastWnd").GetValue<UnityEngine.GameObject>();
                if (mLastWnd != null && mLastWnd != mWorkShopCtrl.gameObject)
                {
                    mLastWnd.SetActive(true);
                }
                else
                {
                    __instance.mRoomWnd.SetActive(true);
                }
                UnityEngine.GameObject.Destroy(mWorkShopCtrl.gameObject);
                Traverse.Create(__instance).Field("mWorkShopCtrl").SetValue(null);
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("BtnJoinOnClick")]
        public static bool BtnJoinOnClick(UILobbyMainWndCtrl __instance)
        {
            var server = new ServerRegistered();

            Traverse.Create(server).Field("IPAddress").SetValue(__instance.mRoomInput.text);
            Traverse.Create(server).Field("Port").SetValue(9900);
            Traverse.Create(server).Field("GameMode").SetValue(0);
            Traverse.Create(server).Field("ServerUID").SetValue(1L);

            Traverse.Create(__instance).Field("mSelectServerData").SetValue(server);
            Traverse.Create(__instance).Field("roomUID").SetValue(1L);

            __instance.mCheckPasswordInput.text = string.Empty;

            return true;
        }
    }
}
