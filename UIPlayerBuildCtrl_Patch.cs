using HarmonyLib;
using System.Linq;

namespace PEPatch
{
    [HarmonyPatch(typeof(UIPlayerBuildCtrl))]
    public class UIPlayerBuildCtrl_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreatePlayer")]
        public static bool CreatePlayer(UIPlayerBuildCtrl __instance)
        {
            if (Pathea.PeGameMgr.IsMulti)
            {
                if (__instance.actionOk)
                {
                    __instance.actionOk = false;
                    __instance.Invoke("ResetActionOK", 2.0f);

                    var name = Traverse.Create(__instance).Field("mNameInput").Property("text").GetValue<string>();

                    if (!string.IsNullOrEmpty(name))
                    {
                        if (GameClientLobby.Self.myRoles.Any(r => r.deletedFlag ==0 && r.name == name))
                        {
                            MessageBox_N.ShowOkBox(PELocalization.GetString(ErrorMessage.NAME_HAS_EXISTED));
                            return false;    
                        }

                        var playerModel = Traverse.Create(__instance).Field("mCurrent").GetValue<PlayerModel>();
                        byte[] appearData = playerModel.mAppearData.Serialize();
                        byte[] nudeData = playerModel.mNude.Serialize();

                        var steamId = SteamFriendPrcMgr.Instance.GetMyInfo()._SteamID.m_SteamID;

                        var roleId = (steamId.GetHashCode() ^ name.GetHashCode()) % 13000000;

                        var role = new CustomData.RoleInfo()
                        {
                            appearData = appearData,
                            nudeData = nudeData,
                            name = name,
                            sex = (byte)(int)Traverse.Create(__instance).Property("Sex").GetValue(),
                            steamId = steamId,
                            roleID = roleId
                        };

                        GameClientLobby.Self.myRoles.Add(role);
                        GameClientLobby.Self.myRolesExisted.Add(role);

                        PatchUtils.SaveMultiplayerData();

                        MLPlayerInfo.Instance.SetSelectedRole(role.name);

                        PeSceneCtrl.Instance.GotoMultiRoleScene();

                        return false;
                    }
                }
            }

            return true;
        }

    }
}
