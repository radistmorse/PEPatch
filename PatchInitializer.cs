using HarmonyLib;
using System.Threading;

namespace PEPatch
{
    public class PatchInitializer
    {
        public static void Main()
        {
            new Thread(() =>
            {
                Thread.Sleep(3000);

                var harmony = new Harmony("PEPatch");

                harmony.PatchAll();

                // Disable the extra steam manager
                var obj = System.Activator.CreateInstance(typeof(SteamProcessMgr).Assembly.GetType("SteamManager"));
                Traverse.Create(obj).Field("m_bInitialized").SetValue(true);
                Traverse.Create(typeof(SteamProcessMgr).Assembly.GetType("SteamManager")).Field("s_instance").SetValue(obj);
            }).Start();
        }
    }
}
