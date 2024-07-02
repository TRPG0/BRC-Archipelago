using HarmonyLib;
using Reptile;
using TMPro;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(VersionUIHandler), "SetVersionText")]
    public class VersionUIHandler_SetVersionText_Patch
    {
        public static void Postfix(VersionUIHandler __instance)
        {
            if (Core.TargetGameVersion.ToString() != Reptile.Core.Instance.GameVersion.ToString())
            {
                Core.Logger.LogWarning($"The current game version ({Reptile.Core.Instance.GameVersion}) does not match the target version. ({Core.TargetGameVersion})");
                Core.Logger.LogWarning("Some things may not work properly.");
            }

            string lineBreak = "\n";
            if (Core.isSlopCrewLoaded) lineBreak += "\n";

            __instance.versionText.text = "ARCHIPELAGO: " + Core.PluginVersion + lineBreak + __instance.versionText.text;
            __instance.versionText.alignment = TextAlignmentOptions.BottomLeft;
        }
    }
}
