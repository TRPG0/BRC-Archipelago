using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(OptionsMenuGameTab), "ApplyLanguage")]
    public class OptionsMenuGameTab_ApplyLanguage_Patch
    {
        public static void Postfix()
        {
            Core.Instance.RandoLocalizer.UpdateLocalization(Reptile.Core.Instance.Localizer.Language);
            Core.Instance.UIManager.UpdateLanguage();
            if (Core.Instance.stageManager != null) Core.Instance.PhoneManager.UpdateLanguage();
        }
    }
}
