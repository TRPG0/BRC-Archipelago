using Archipelago.Structures;
using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), "GetRawDialogueTextValue")]
    public class TextMeshProGameTextLocalizer_GetRawDialogueTextValue_Patch
    {
        public static void Postfix(string localizationKey, ref string __result)
        {
            if (Reptile.Core.Instance.BaseModule.IsPlayingInStage && Core.Instance.SaveManager.DataExists())
            {
                RepValues values = Requirements.GetLocalizedTextRepValues(localizationKey);
                if (values == null) return;

                if (localizationKey == "Dialogue_tower_82") __result = __result.Replace((values.oldValue - 1).ToString(), (values.newValue - 1).ToString());
                else __result = __result.Replace(values.oldValue.ToString(), values.newValue.ToString());
                Core.Logger.LogInfo($"Changed dialogue with key \"{localizationKey}\" by replacing \"{values.oldValue}\" with \"{values.newValue}\".");
            }
        }
    }
}
