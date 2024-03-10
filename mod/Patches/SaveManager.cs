using HarmonyLib;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(Reptile.SaveManager), "SaveCurrentSaveSlot")]
    public class SaveManager_SaveCurrentSaveSlot_Patch
    {
        public static void Prefix()
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                Core.Instance.SaveManager.SaveData();
            }
        }
    }
}
