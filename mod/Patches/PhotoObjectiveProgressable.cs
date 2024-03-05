using HarmonyLib;
using Reptile;
using Reptile.Phone;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(PhotoObjectiveProgressable), "MadePhotograph")]
    public class PhotoObjectiveProgressable_MadePhotograph_Patch
    {
        public static void Postfix(PhotoObjectiveProgressable __instance)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                string loc = $"{__instance.transform.parent.name}/{__instance.name}";

                if (!Core.Instance.LocationManager.locations[loc].@checked && Traverse.Create(Core.Instance.PhoneManager.Phone).Method("IsCurrentAppAndOpen", new object[] { typeof(AppCamera) }).GetValue<bool>())
                {
                    Core.Instance.PhoneManager.Phone.CloseCurrentApp();
                }

                Core.Instance.LocationManager.CheckLocation(loc, false);
            }
        }
    }
}
