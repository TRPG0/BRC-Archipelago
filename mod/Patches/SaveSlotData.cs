using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(SaveSlotData), "UnlockCharacter")]
    public class SaveSlotData_UnlockCharacter_Patch
    {
        public static bool Prefix(ref Characters character)
        {
            if (Core.Instance.SaveManager.DataExists() && 
                !(character == Characters.metalHead || character == Characters.legendMetalHead || character == Characters.legendFace
                || (character == Characters.dummy && Core.Instance.Data.dummyUnlocked)))
            {
                Core.Instance.LocationManager.CheckLocation(character.ToString());
                return false;
            }
            else return true;
        }
    }
}
