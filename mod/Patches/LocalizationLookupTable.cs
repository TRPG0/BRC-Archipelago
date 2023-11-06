using Archipelago.Structures;
using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(LocalizationLookupTable), "GetLocalizationValueFromSubgroup")]
    public class LocalizationLookupTable_GetLocalizationValueFromSubgroup_Patch
    {
        public static void Postfix(string groupName, string localizationKey, ref string __result)
        {
            if (Reptile.Core.Instance.BaseModule.IsPlayingInStage && Core.Instance.SaveManager.DataExists() && groupName == "Notifications")
            {
                RepValues values = Requirements.GetLocalizedTextRepValues(localizationKey);
                if (values == null) return;

                __result = __result.Replace(values.oldValue.ToString(), values.newValue.ToString());
                Core.Logger.LogInfo($"Changed notification with key \"{localizationKey}\" by replacing \"{values.oldValue}\" with \"{values.newValue}\".");
            }
        }
    }
}
