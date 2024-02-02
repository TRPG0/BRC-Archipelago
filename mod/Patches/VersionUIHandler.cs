using HarmonyLib;
using Reptile;
using System;
using TMPro;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(VersionUIHandler), "SetVersionText")]
    public class VersionUIHandler_SetVersionText_Patch
    {
        public static void Postfix(VersionUIHandler __instance)
        {
            __instance.versionText.text = "ARCHIPELAGO: " + Core.PluginVersion + " (prerelease 5)\n" + __instance.versionText.text;
            __instance.versionText.alignment = TextAlignmentOptions.BottomLeft;
        }
    }
}
