using HarmonyLib;
using Reptile;
using Reptile.Phone;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(PhoneScrollUnlockableButton), "SetContent")]
    public class PhoneScrollUnlockableButton_SetContent_Patch
    {
        public static bool Prefix(AUnlockable content, PhoneScrollUnlockableButton __instance)
        {
            if (content is GraffitiAppEntry graffiti && graffiti.Size == GraffitiSize.S)
            {
                Traverse.Create(__instance).Field<AUnlockable>("<AssignedContent>k__BackingField").Value = graffiti;
                return false;
            }
            else return true;
        }
    }
}
