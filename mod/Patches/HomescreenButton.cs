using HarmonyLib;
using ModLocalizer;
using Reptile.Phone;
using TMPro;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(HomescreenButton), "SetContent")]
    public class HomescreenButton_SetContent_Patch
    {
        public static void Postfix(HomescreenButton __instance)
        {
            if (__instance.AssignedApp.AppName == "AppArchipelago" || __instance.AssignedApp.AppName == "AppEncounter")
            {
                Traverse.Create(__instance).Field<TextMeshProUGUI>("m_TitleLabel").Value.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, __instance.AssignedApp.DisplayName);
            }
        }
    }
}
