using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(Collectable), "PickupCollectable")]
    public class Collectable_PickupCollectable_Patch
    {
        public static bool Prefix(Collectable __instance)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                Core.Instance.LocationManager.CheckLocation(__instance.unlock.name);
                Traverse.Create(__instance).Field<bool>("pickedUp").Value = true;
                __instance.gameObject.SetActive(false);
                __instance.WriteToData();
                return false;
            }
            else return true;
        }
    }
}
