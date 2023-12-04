using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(ScoreEncounter), "StartMainEvent")]
    public class ScoreEncounter_StartMainEvent_Patch
    {
        public static void Postfix(ScoreEncounter __instance)
        {
            if (Core.FailScoreEncounters) Traverse.Create(__instance).Field<bool>("hasOpponents").Value = true;
        }
    }
}
