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
            }).Start();
        }
    }
}
