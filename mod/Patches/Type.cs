using HarmonyLib;
using Archipelago.Apps;
using System;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(Type))]
    public class Type_GetType_Patch
    {
        // look I might not be the smartest or most experienced dev but even I can tell that having to do this just to open custom apps is insane
        [HarmonyPatch("GetType", typeof(string))]
        public static void Postfix(string typeName, ref Type __result)
        {
            if (typeName == "Reptile.Phone.AppArchipelago") __result = typeof(AppArchipelago);
            else if (typeName == "Reptile.Phone.AppEncounter") __result = typeof(AppEncounter);
        }
    }
}
