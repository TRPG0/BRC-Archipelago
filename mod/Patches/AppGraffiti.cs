using HarmonyLib;
using Reptile.Phone;

namespace Archipelago.Patches
{
    // prevent crashes when graffiti list is empty
    [HarmonyPatch(typeof(AppGraffiti), "OnHoldDown")]
    public class AppGraffiti_OnHoldDown_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (__instance.GraffitiArt.Count == 0) return false;
            else return true;
        }
    }

    [HarmonyPatch(typeof(AppGraffiti), "OnHoldUp")]
    public class AppGraffiti_OnHoldUp_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (__instance.GraffitiArt.Count == 0) return false;
            else return true;
        }
    }

    [HarmonyPatch(typeof(AppGraffiti), "OnPressDown")]
    public class AppGraffiti_OnPressDown_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (__instance.GraffitiArt.Count == 0) return false;
            else return true;
        }
    }

    [HarmonyPatch(typeof(AppGraffiti), "OnPressRight")]
    public class AppGraffiti_OnPressRight_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (__instance.GraffitiArt.Count == 0) return false;
            else return true;
        }
    }

    [HarmonyPatch(typeof(AppGraffiti), "OnPressUp")]
    public class AppGraffiti_OnPressUp_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (__instance.GraffitiArt.Count == 0) return false;
            else return true;
        }
    }

    [HarmonyPatch(typeof(AppGraffiti), "OnReleaseRight")]
    public class AppGraffiti_OnReleaseRight_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (__instance.GraffitiArt.Count == 0) return false;
            else return true;
        }
    }
}
