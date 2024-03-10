using HarmonyLib;
using Reptile;
using Reptile.Phone;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(GraffitiInfoPanel), "Show")]
    public class GraffitiInfoPanel_Show_Patch
    {
        public static bool Prefix(GraffitiAppEntry content)
        {
            if (content.Size == GraffitiSize.S) return false;
            else return true;
        }
    }
}
