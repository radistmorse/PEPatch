using HarmonyLib;

namespace PEPatch
{
    [HarmonyPatch(typeof(UIMLoginControl))]
    public class UIMLoginControl_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("EnterLobby")]
        public static bool EnterLobby(UIMLoginControl __instance)
        {
            Traverse.Create<GameClientLobby>().Property("role").SetValue(MLPlayerInfo.Instance.GetRoleInfo(__instance.rc.GetSelectedIndex()).mRoleInfo);
            PeSceneCtrl.Instance.GotoLobbyScene();

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("BtnOKOnClick")]
        public static bool BtnOKOnClick(UIMLoginControl __instance)
        {
            __instance.BtnOK?.Invoke();
            var roleId = Traverse.Create(__instance).Field("deleteRoleIndex").GetValue<int>();

            CustomData.RoleInfo role = MLPlayerInfo.Instance.GetRoleInfo(__instance.rc.GetSelectedIndex()).mRoleInfo;
            role.deletedFlag = 1;
            GameClientLobby.Self.myRolesExisted.Remove(role);
            GameClientLobby.Self.myRolesDeleted.Clear();
            GameClientLobby.Self.myRolesDeleted.Add(role);
            MLPlayerInfo.Instance.DeleteRole(roleId);

            PatchUtils.SaveMultiplayerData();

            return false;
        }


    }
}
