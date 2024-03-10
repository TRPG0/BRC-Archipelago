using HarmonyLib;
using Reptile;
using Reptile.Phone;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(PhotoObjectiveProgressable), "MadePhotograph")]
    public class PhotoObjectiveProgressable_MadePhotograph_Patch
    {
        public static void Postfix(PhotoObjectiveProgressable __instance)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                string loc = $"{__instance.transform.parent.name}/{__instance.name}";

                if (!Core.Instance.Data.@checked.Contains(loc) && Traverse.Create(Core.Instance.PhoneManager.Phone).Method("IsCurrentAppAndOpen", new object[] { typeof(AppCamera) }).GetValue<bool>())
                {
                    Core.Instance.PhoneManager.Phone.CloseCurrentApp();
                }

                Core.Instance.LocationManager.CheckLocation(loc, false);
            }
        }
    }
}
