using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;

namespace PEPatch
{
    [HarmonyPatch(typeof(UILobbyMainWndCtrl))]
    public class UILobbyMainWndCtrl_Patch
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

            SteamServerHandler.currentLanRequest =
                SteamMatchmakingServers.RequestLANServerList(new AppId_t(237870u),
                    new ISteamMatchmakingServerListResponse(SteamServerHandler.ServerResponded, SteamServerHandler.ServerFailedToRespond, SteamServerHandler.RefreshComplete));
            SteamServerHandler.currentInternetRequest =
                SteamMatchmakingServers.RequestInternetServerList(new AppId_t(237870u), new MatchMakingKeyValuePair_t[] { new MatchMakingKeyValuePair_t() { m_szKey = "gametagsand", m_szValue = "public" } }, 1,
                    new ISteamMatchmakingServerListResponse(SteamServerHandler.ServerResponded, SteamServerHandler.ServerFailedToRespond, SteamServerHandler.RefreshComplete));
            SteamServerHandler.lastUpdate = true;


            __instance.GetType().GetMethod("InitRoomListSort", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { });


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
            if (__instance.mPlayerInput.text != null)
            {
                var nums = __instance.mPlayerInput.text.Split(',');
                ulong steamid = 0ul;
                if (int.TryParse(nums[0], out var roleid) && roleid < 13000000 && (nums.Length == 1 || (nums.Length == 2 && ulong.TryParse(nums[1], out steamid))))
                {
                    Traverse.Create<GameClientLobby>().Property("role").GetValue<CustomData.RoleInfo>().roleID = roleid;
                    if (nums.Length == 2)
                    {
                        Traverse.Create<GameClientLobby>().Property("role").GetValue<CustomData.RoleInfo>().steamId = steamid;
                    }
                }
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("BtnRefreshOnClick")]
        public static bool BtnRefreshOnClick(UILobbyMainWndCtrl __instance)
        {
            if (__instance.mRoomListPage != 0 && __instance.mRoomListPage != 1)
            {
                return false;
            }

            var lan = __instance.mRoomListPage == 1;

            var request = SteamServerHandler.currentInternetRequest;
            if (lan)
            {
                request = SteamServerHandler.currentLanRequest;
            }
            if (request == HServerListRequest.Invalid || !SteamMatchmakingServers.IsRefreshing(request))
            {
                if (lan)
                {
                    Traverse.Create(__instance).Field("_serverListLan").GetValue<List<ServerRegistered>>().Clear();
                }
                else
                {
                    Traverse.Create(__instance).Field("_serverListInter").GetValue<List<ServerRegistered>>().Clear();
                }
                typeof(UILobbyMainWndCtrl).GetMethod("RefreshRoomList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(__instance, new object[] { });


                if (request != HServerListRequest.Invalid)
                {
                    SteamMatchmakingServers.RefreshQuery(request);
                }
                else
                {
                    if (lan)
                    {
                        SteamServerHandler.currentLanRequest =
                            SteamMatchmakingServers.RequestLANServerList(new AppId_t(237870u),
                                new ISteamMatchmakingServerListResponse(SteamServerHandler.ServerResponded, SteamServerHandler.ServerFailedToRespond, SteamServerHandler.RefreshComplete));
                    }
                    else
                    {
                        SteamServerHandler.currentInternetRequest =
                            SteamMatchmakingServers.RequestInternetServerList(new AppId_t(237870u), new MatchMakingKeyValuePair_t[] { new MatchMakingKeyValuePair_t() { m_szKey = "gametagsand", m_szValue = "public" } }, 1,
                                new ISteamMatchmakingServerListResponse(SteamServerHandler.ServerResponded, SteamServerHandler.ServerFailedToRespond, SteamServerHandler.RefreshComplete));
                    }
                }
                SteamServerHandler.lastUpdate = true;
            }
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("UpdateRoomInfo")]
        public static bool UpdateRoomInfo(ref IEnumerator __result)
        {
            __result = SteamServerHandler.UpdateRoomInfo();
            return false;
        }
    }

    public static class SteamServerHandler
    {
        internal static HServerListRequest currentInternetRequest = HServerListRequest.Invalid;
        internal static HServerListRequest currentLanRequest = HServerListRequest.Invalid;

        internal static void ServerResponded(HServerListRequest hRequest, int iServer)
        {
            if (UILobbyMainWndCtrl.Instance != null)
            {
                var details = SteamMatchmakingServers.GetServerDetails(hRequest, iServer);
                var tags = details.GetGameTags()?.Split(',');
                if (tags != null && (tags.Length == 6 || tags.Length == 7))
                {
                    var server = new ServerRegisteredLocal()
                    {
                        PasswordStatus = details.m_bPassword ? 1 : 0,
                        ServerUID = 0L,
                        ServerID = iServer,
                        ServerName = details.GetServerName(),
                        ServerMasterName = string.Empty,
                        CurConn = 0,
                        LimitedConn = details.m_nMaxPlayers,
                        GameType = 0,
                        GameMode = 0,
                        Ping = details.m_nPing,
                        ServerStatus = details.m_nBotPlayers,
                        ServerVersion = details.m_nServerVersion.ToString(),
                        MapName = details.GetMap(),

                        IsLan = false,
                        IPAddress = details.m_NetAdr.GetConnectionAddressString().Split(':')[0],
                        Port = details.m_NetAdr.GetConnectionPort()
                    };

                    server.GameType = (int)(Pathea.PeGameMgr.EGameType)Enum.Parse(typeof(Pathea.PeGameMgr.EGameType), tags[0]);
                    server.GameMode = (int)(Pathea.PeGameMgr.ESceneMode)Enum.Parse(typeof(Pathea.PeGameMgr.ESceneMode), tags[1]);
                    server.ServerUID = long.Parse(tags[2], System.Globalization.NumberStyles.HexNumber);
                    server.ServerStatus = int.Parse(tags[3]);
                    server.CurConn = int.Parse(tags[4]);
                    server.ServerMasterName = tags[5];
                    if (hRequest == currentInternetRequest)
                    {
                        Traverse.Create(UILobbyMainWndCtrl.Instance).Field("_serverListInter").GetValue<List<ServerRegistered>>().Add(server.ToServerRegistered());
                    }
                    if (hRequest == currentLanRequest)
                    {
                        Traverse.Create(UILobbyMainWndCtrl.Instance).Field("_serverListLan").GetValue<List<ServerRegistered>>().Add(server.ToServerRegistered());
                    }
                }
            }
        }

        internal static void ServerFailedToRespond(HServerListRequest hRequest, int iServer)
        {
        }

        internal static void RefreshComplete(HServerListRequest hRequest, EMatchMakingServerResponse response)
        {
            if (currentInternetRequest == hRequest)
            {
                UnityEngine.Debug.Log($"[STEAMWORKS] Internet RefreshComplete: {response}, num = {SteamMatchmakingServers.GetServerCount(hRequest)}");
                currentInternetRequest = HServerListRequest.Invalid;
                SteamMatchmakingServers.ReleaseRequest(hRequest);
            }
            if (currentLanRequest == hRequest)
            {
                UnityEngine.Debug.Log($"[STEAMWORKS] LAN RefreshComplete: {response}, num = {SteamMatchmakingServers.GetServerCount(hRequest)}");
                currentLanRequest = HServerListRequest.Invalid;
                SteamMatchmakingServers.ReleaseRequest(hRequest);
            }
        }

        public static bool lastUpdate = false;

        internal static IEnumerator UpdateRoomInfo()
        {
            while (true)
            {
                if (UILobbyMainWndCtrl.Instance != null)
                {
                    var refreshing = (currentInternetRequest != HServerListRequest.Invalid && SteamMatchmakingServers.IsRefreshing(currentInternetRequest))
                                  || (currentLanRequest != HServerListRequest.Invalid && SteamMatchmakingServers.IsRefreshing(currentLanRequest));

                    if (refreshing || lastUpdate)
                    {
                        typeof(UILobbyMainWndCtrl).GetMethod("RefreshRoomList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(UILobbyMainWndCtrl.Instance, new object[] { });
                        lastUpdate = refreshing;
                    }
                }

                yield return new UnityEngine.WaitForSeconds(5f);
            }
        }
    }
}
