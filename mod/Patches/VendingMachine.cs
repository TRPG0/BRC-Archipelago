using HarmonyLib;
using Reptile;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(VendingMachine), "RewardIsValid")]
    public class VendingMachine_RewardIsValid_Patch
    {
        public static bool Prefix(VendingMachine __instance, ref bool __result)
        {
            if (Core.Instance.SaveManager.DataExists() && __instance.unlockableDrop != null)
            {
                if (!Core.Instance.Data.@checked.Contains(__instance.unlockableDrop.GetComponent<DynamicPickup>().unlock.name)) __result = true;
                else __result = false;

                return false;
            }
            else return true;
        }
    }
}
