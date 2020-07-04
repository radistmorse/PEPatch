using HarmonyLib;

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
                    var name = Traverse.Create(__instance).Field("mNameInput").Property("text").GetValue<string>();

                    if (!string.IsNullOrEmpty(name))
                    {
                        var playerModel = Traverse.Create(__instance).Field("mCurrent").GetValue<PlayerModel>();
                        byte[] appearData = playerModel.mAppearData.Serialize();
                        byte[] nudeData = playerModel.mNude.Serialize();
                        var role = new CustomData.RoleInfo()
                        {
                            appearData = appearData,
                            nudeData = nudeData,
                            name = name,
                            sex = (byte)(int)Traverse.Create(__instance).Property("Sex").GetValue()
                        };

                        GameClientLobby.Self.myRoles.Add(role);
                        GameClientLobby.Self.myRolesExisted.Add(role);
                        role.roleID = GameClientLobby.Self.myRoles.Count;

                        PatchUtils.SaveMultiplayerData();

                        MLPlayerInfo.Instance.SetSelectedRole(role.name);
                        MLPlayerInfo.Instance.UpdateScene();


                        PeSceneCtrl.Instance.GotoMultiRoleScene();

                        return false;
                    }
                }
            }

            return true;
        }

    }
}
