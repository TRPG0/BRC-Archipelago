using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(SaveSlotData), "UnlockCharacter")]
    public class SaveSlotData_UnlockCharacter_Patch
    {
        public static bool Prefix(ref Characters character)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                Core.Instance.LocationManager.CheckLocation(character.ToString());
                return false;
            }
            else return true;
        }
    }
}
