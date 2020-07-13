using HarmonyLib;
using System.Threading;

namespace PEServerPatch
{
    public class PatchInitializer
    {
        public static void Main()
        {
            new Thread(() =>
            {
                Thread.Sleep(100);

                var harmony = new Harmony("PEServerPatch");

                harmony.PatchAll();
            }).Start();
        }
    }
}
