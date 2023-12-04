using HarmonyLib;
using Reptile;
using System;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(GameplayEvent), "UnlockCharacterImpl")]
    public class GameplayEvent_UnlockCharacterImpl_Patch
    {
        public static bool Prefix(string character, bool showNotification)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                Characters characters = (Characters)Enum.Parse(typeof(Characters), character);
                Core.Instance.SaveManager.CurrentSaveSlot.UnlockCharacter(characters);
                return false;
            }
            else return true;
        }
    }
}
