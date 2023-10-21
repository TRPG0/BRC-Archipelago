using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(Player), "ChangeHP")]
    public class Player_ChangeHP_Patch
    {
        public static void Prefix(ref int dmg)
        {
            if (Core.Instance.SaveManager.DataExists() && dmg > 0)
            {
                dmg *= Core.Instance.Data.damageMultiplier;
            }
        }
    }

    [HarmonyPatch(typeof(Player), "StartGraffitiMode")]
    public class Player_StartGraffitiMode_Patch
    {
        public static bool Prefix()
        {
            if (Core.Instance.Data.limitedGraffiti)
            {
                Encounter currentEncounter = Traverse.Create(WorldHandler.instance).Field<Encounter>("currentEncounter").Value;
                if (currentEncounter != null)
                {
                    if (currentEncounter.name.Contains("CrewBattle"))
                    {
                        return false;
                    }
                    return true;
                }
                return true;
            }
            else return true;
        }
    }
}
