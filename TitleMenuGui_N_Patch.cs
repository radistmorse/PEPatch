using HarmonyLib;

namespace PEPatch
{
    [HarmonyPatch(typeof(TitleMenuGui_N))]
    public class TitleMenuGui_N_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch("Update")]
        public static bool Update(TitleMenuGui_N __instance)
        {
            if (SystemSettingData.Instance != null)
            {
                if (__instance.mControlWnd.activeSelf)
                {
                    if (SystemSettingData.Instance.FixBlurryFont != __instance.mFixBlurryCB.isChecked)
                    {
                        SystemSettingData.Instance.FixBlurryFont = __instance.mFixBlurryCB.isChecked;
                        if (UIFontMgr.Instance != null)
                        {
                            UILabel[] labels = UIFontMgr.Instance.gameObject.GetComponentsInChildren<UILabel>(true);
                            for (int i = 0; i < labels.Length; i++)
                            {
                                labels[i].MakePixelPerfect();
                            }
                        }


                    }
                }
            }

            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("OnMultiplayerBtn")]
        public static bool OnMultiplayerBtn(TitleMenuGui_N __instance)
        {
            Pathea.PeGameMgr.playerType = Pathea.PeGameMgr.EPlayerType.Multiple;
            RandomMapConfig.useSkillTree = false;

            PatchUtils.LoadMultiplayerData();

            PeSceneCtrl.Instance.GotoMultiRoleScene();

            return false;
        }
    }
}
