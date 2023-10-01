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
}
