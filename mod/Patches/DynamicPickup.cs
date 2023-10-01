using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(DynamicPickup), "PickupPickup")]
    public class DynamicPickup_PickupPickup_Patch
    {
        public static bool Prefix(DynamicPickup __instance)
        {
            if (Core.Instance.SaveManager.DataExists() && __instance.unlock != null)
            {
                Core.Instance.LocationManager.CheckLocation(__instance.unlock.name);
                Traverse.Create(__instance).Field<bool>("pickedUp").Value = true;
                __instance.gameObject.SetActive(false);
                return false;
            }
            else return true;
        }
    }
}
