using HarmonyLib;
using Reptile;
using System.Collections.Generic;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(CharacterSelect), "PopulateListOfSelectableCharacters")]
    public class CharacterSelect_PopulateListOfSelectableCharacters_Patch
    {
        public static void Postfix(CharacterSelect __instance)
        {
            List<Characters> list = Traverse.Create(__instance).Field<List<Characters>>("selectableCharacters").Value;

            if (list.Count == 1) list.Add(list[0]);
        }
    }
}
