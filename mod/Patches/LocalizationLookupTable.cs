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
                Values values = Requirements.GetLocalizedTextRepValues(localizationKey);
                if (values is RepValues repValues)
                {
                    __result = __result.Replace(repValues.oldValue.ToString(), repValues.newValue.ToString());
                    Core.Logger.LogInfo($"Changed notification with key \"{localizationKey}\" by replacing \"{repValues.oldValue}\" with \"{repValues.newValue}\".");
                }
            }
        }
    }
}
