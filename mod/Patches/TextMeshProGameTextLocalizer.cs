using Archipelago.Structures;
using HarmonyLib;
using Reptile;
using System.Globalization;
using UnityEngine;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(TextMeshProGameTextLocalizer), "GetRawDialogueTextValue")]
    public class TextMeshProGameTextLocalizer_GetRawDialogueTextValue_Patch
    {
        public static void Postfix(string localizationKey, ref string __result)
        {
            if (Reptile.Core.Instance.BaseModule.IsPlayingInStage && Core.Instance.SaveManager.DataExists())
            {
                Values values = Requirements.GetLocalizedTextRepValues(localizationKey);
                if (values is RepValues repValues)
                {
                    if (localizationKey == "Dialogue_tower_82") __result = __result.Replace((repValues.oldValue - 1).ToString(), (repValues.newValue - 1).ToString());
                    else __result = __result.Replace(repValues.oldValue.ToString(), repValues.newValue.ToString());
                    Core.Logger.LogInfo($"Changed dialogue with key \"{localizationKey}\" by replacing \"{repValues.oldValue}\" with \"{repValues.newValue}\".");
                }
                else if (values is ScoreValues scoreValues && Core.Instance.Data.scoreDifficulty > ScoreDifficulty.Normal)
                {
                    int oldValue = scoreValues.oldValue;
                    int newValue = scoreValues.oldValue;

                    if (Core.Instance.Data.scoreDifficulty == ScoreDifficulty.Hard) newValue = scoreValues.hardValue;
                    if (Core.Instance.Data.scoreDifficulty == ScoreDifficulty.VeryHard) newValue = scoreValues.veryHardValue;
                    if (Core.Instance.Data.scoreDifficulty == ScoreDifficulty.Extreme) newValue = scoreValues.extremeValue;

                    CultureInfo info = new CultureInfo("en-US", false);
                    SystemLanguage currentLanguage = Traverse.Create(Reptile.Core.Instance).Field<TextMeshProGameTextLocalizer>("gameTextLocalizer").Value.Language;
                    if (currentLanguage == SystemLanguage.French || currentLanguage == SystemLanguage.Russian || currentLanguage == SystemLanguage.Spanish) info.NumberFormat.NumberGroupSeparator = " ";
                    else if (currentLanguage == SystemLanguage.Japanese) info.NumberFormat.NumberGroupSeparator = ",";
                    else info.NumberFormat.NumberGroupSeparator = ".";

                    if (localizationKey == "Dialogue_Mall_86")
                    {
                        oldValue = oldValue - 5000;
                        newValue = newValue - 5000;
                    }
                    else if (localizationKey == "Dialogue_Mall_87")
                    {
                        oldValue = oldValue - 4000;
                        newValue = newValue - 4000;
                    }
                    else if (localizationKey == "Dialogue_Mall_88")
                    {
                        oldValue = oldValue - 3000;
                        newValue = newValue - 3000;
                    }
                    else if (localizationKey == "Dialogue_Mall_89")
                    {
                        oldValue = 98;
                        newValue = newValue / 1000 - 2;
                    }
                    else if (localizationKey == "Dialogue_Mall_92")
                    {
                        oldValue = oldValue - 2000;
                        newValue = newValue - 2000;
                    }

                    __result = __result.Replace(oldValue.ToString("N0", info), newValue.ToString("N0", info));
                    Core.Logger.LogInfo($"Changed dialogue with key \"{localizationKey}\" by replacing \"{oldValue.ToString("N0", info)}\" with \"{newValue.ToString("N0", info)}\".");
                }
            }
        }
    }
}
