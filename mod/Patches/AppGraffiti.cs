using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using UnityEngine;
using Archipelago.Structures;
using System;

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
            Traverse traverse = Traverse.Create(__instance);
            if (__instance.GraffitiArt.Count == 0) return false;
            else if (__instance.GraffitiArt[traverse.Field<GraffitiScrollView>("m_ScrollView").Value.GetContentIndex()].Size == GraffitiSize.S) return false;
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

    [HarmonyPatch(typeof(AppGraffiti), "OpenInfo")]
    public class AppGraffiti_OpenInfo_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            Traverse traverse = Traverse.Create(__instance);
            if (__instance.GraffitiArt[traverse.Field<GraffitiScrollView>("m_ScrollView").Value.GetContentIndex()].Size == GraffitiSize.S) return false;
            else return true;
        }
    }

    [HarmonyPatch(typeof(AppGraffiti), "RefreshList")]
    public class AppGraffiti_RefreshList_Patch
    {
        public static bool Prefix(AppGraffiti __instance)
        {
            if (Core.Instance.SaveManager.DataExists() && Core.Instance.Data.limitedGraffiti)
            {
                int num = 0;
                if (__instance.GraffitiArt != null) num = __instance.GraffitiArt.Count;
                Traverse traverse = Traverse.Create(__instance);
                List<GraffitiAppEntry> graffitiArt = new List<GraffitiAppEntry>();

                if (Core.Instance.Data.sGraffiti == SGraffiti.Separate)
                {
                    foreach (CharacterProgress progress in Traverse.Create(Core.Instance.SaveManager.CurrentSaveSlot).Field<CharacterProgress[]>("totalCharacterProgress").Value)
                    {
                        if (progress.unlocked && Core.Instance.Data.grafUses.ContainsKey(progress.character.ToString()))
                        {
                            if (Core.Instance.Data.grafUses[progress.character.ToString()] < Requirements.grafSLimit)
                            {
                                GraffitiArt ga = WorldHandler.instance.graffitiArtInfo.FindByCharacter(progress.character);
                                GraffitiAppEntry unlockable = ScriptableObject.CreateInstance<GraffitiAppEntry>();
                                unlockable.name = "S";
                                unlockable.Artist = ga.artistName;
                                unlockable.Title = ga.title;
                                unlockable.Size = ga.graffitiSize;
                                unlockable.Combos = GraffitiArt.Combos.NONE;
                                unlockable.Uid = progress.character.ToString();
                                unlockable.GraffitiTexture = ga.graffitiMaterial.mainTexture;
                                graffitiArt.Add(unlockable);
                            }
                        }
                    }
                }
                else
                {
                    if (Core.Instance.Data.sMax - Core.Instance.Data.grafUses["S"] > 0)
                    {
                        GraffitiArt ga = WorldHandler.instance.graffitiArtInfo.FindByCharacter(Core.Instance.SaveManager.CurrentSaveSlot.currentCharacter);
                        GraffitiAppEntry unlockable = ScriptableObject.CreateInstance<GraffitiAppEntry>();
                        unlockable.name = "S";
                        unlockable.Artist = ga.artistName;
                        unlockable.Title = ga.title;
                        unlockable.Size = ga.graffitiSize;
                        unlockable.Combos = GraffitiArt.Combos.NONE;
                        unlockable.Uid = "S";
                        unlockable.GraffitiTexture = ga.graffitiMaterial.mainTexture;
                        graffitiArt.Add(unlockable);
                    }
                }
                
                for (int i = 0; i < __instance.Unlockables.Length; i++)
                {
                    if (Reptile.Core.Instance.Platform.User.GetUnlockableSaveDataFor(__instance.Unlockables[i]).IsUnlocked)
                    {
                        graffitiArt.Add(__instance.Unlockables[i] as GraffitiAppEntry);
                    }
                }
                graffitiArt.Sort(delegate (GraffitiAppEntry a, GraffitiAppEntry b)
                {
                    int num2 = a.Size.CompareTo(b.Size);
                    if (num2 == 0) return a.Artist.CompareTo(b.Artist);
                    return num2;
                });
                traverse.Field<List<GraffitiAppEntry>>("<GraffitiArt>k__BackingField").Value = graffitiArt;
                if (num == 0)
                {
                    traverse.Field<GraffitiScrollView>("m_ScrollView").Value.SetListContent(__instance.GraffitiArt.Count);
                    return false;
                }
                traverse.Field<GraffitiScrollView>("m_ScrollView").Value.UpdateListContent(__instance.GraffitiArt.Count);
                return false;
            }
            else return true;
        }

        public static void Postfix(AppGraffiti __instance)
        {
            foreach (PhoneScrollButton button in Traverse.Create(__instance).Field<GraffitiScrollView>("m_ScrollView").Value.GetButtons())
            {
                PhoneScrollUnlockableButton ubutton = button as GraffitiScrollButton;
                if (ubutton.AssignedContent == null) continue;
                if (ubutton.AssignedContent.name == "S") Traverse.Create(ubutton).Method("SetIndicatorState", new object[] { false }).GetValue();
            }
        }
    }
}
