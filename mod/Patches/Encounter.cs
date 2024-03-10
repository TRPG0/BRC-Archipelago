using HarmonyLib;
using Reptile;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(Encounter), "SetEncounterState")]
    public class Encounter_SetEncounterState_Patch
    {
        public static void Prefix(ref Encounter.EncounterState setState, Encounter __instance)
        {
            if (Core.FailScoreEncounters && __instance is ScoreEncounter && setState == Encounter.EncounterState.MAIN_EVENT_SUCCES_DECAY)
            {
                setState = Encounter.EncounterState.MAIN_EVENT_FAILED_DECAY;
            }
        }
    }
}
