using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System;
using System.Collections.Generic;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(GraffitiGame), "OnDestroy")]
    public class GraffitiGame_OnDestroy_Patch
    {
        public static void Postfix()
        {
            Core.Instance.PhoneManager.Phone.GetAppInstance<AppGraffiti>().OnAppRefresh();
        }
    }

    [HarmonyPatch(typeof(GraffitiGame), "SetState")]
    public class GraffitiGame_SetState_Patch
    {
        public static void Postfix(GraffitiGame __instance, GraffitiGame.GraffitiGameState setState)
        {
            if (Traverse.Create(__instance).Field<GraffitiSpot>("gSpot").Value is GraffitiSpotFinisher) return;
            if (Core.Instance.SaveManager.DataExists() && Core.Instance.Data.limitedGraffiti)
            {
                if (setState == GraffitiGame.GraffitiGameState.SHOW_PIECE)
                {
                    GraffitiSpot gSpot = Traverse.Create(__instance).Field<GraffitiSpot>("gSpot").Value;
                    if (gSpot.attachedTo == GraffitiSpot.AttachType.DEFAULT)
                    {
                        if (gSpot.bottomCrew != gSpot.topCrew && (gSpot.topCrew == Crew.PLAYERS || gSpot.bottomCrew == Crew.ROGUE) && Reptile.Core.Instance.BaseModule.CurrentStage != Stage.Prelude)
                        {
                            GraffitiArt grafArt = Traverse.Create(__instance).Field<GraffitiArt>("grafArt").Value;

                            int limit = Requirements.grafSLimit;
                            if (gSpot.size == GraffitiSize.M) limit = Requirements.grafMLimit;
                            else if (gSpot.size == GraffitiSize.L) limit = Requirements.grafLLimit;
                            else if (gSpot.size == GraffitiSize.XL) limit = Requirements.grafXLLimit;

                            string id = "";
                            string title = "";
                            if (gSpot.size == GraffitiSize.S)
                            {
                                Characters currentCharacter = Core.Instance.SaveManager.CurrentSaveSlot.currentCharacter;
                                Requirements.OverrideCharacterIfInvalid(ref currentCharacter);
                                id = currentCharacter.ToString();
                                title = Reptile.Core.Instance.Localizer.GetCharacterName(currentCharacter);
                            }
                            else
                            {
                                id = grafArt.unlockable.Uid;
                                title = grafArt.title;
                            }

                            Core.Instance.Data.grafUses[id] += 1;
                            if (Core.Instance.Data.grafUses[id] >= limit)
                            {
                                if (gSpot.size != GraffitiSize.S)
                                {
                                    grafArt.unlockable.IsDefault = false;
                                    Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(grafArt.unlockable.Uid).IsUnlocked = false;
                                    Core.Instance.LocationManager.notifQueue.Add(new Structures.Notification("AppGraffiti", string.Format(Core.Instance.Localizer.GetRawTextValue("GRAFFITI_DEPLETED_COLLECTIBLE"), grafArt.title), null));

                                    List<string> defaults = new List<string>()
                                    {
                                    "OVERWHELMME",
                                    "QUICK BING",
                                    "WHOLE SIXER",
                                    "Graffo Le Fou",
                                    "WILD STRUXXA",
                                    "Bombing by FireMan"
                                    };

                                    if (defaults.Contains(grafArt.title)) Core.Instance.Data.to_lock.Add(grafArt.title);
                                    if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(gSpot.size)) Core.Instance.stageManager.NoGraffiti(gSpot.size);
                                }
                                else Core.Instance.LocationManager.notifQueue.Add(new Structures.Notification("AppGraffiti", string.Format(Core.Instance.Localizer.GetRawTextValue("GRAFFITI_DEPLETED_CHARACTER"), title), null));
                            }

                            Core.Instance.SaveManager.SaveData();
                            Core.Logger.LogInfo($"\"{title}\" has been used {Core.Instance.Data.grafUses[id]} / {limit} times.");
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(GraffitiGame), "SetStateVisual")]
    public class GraffitiGame_SetStateVisual_Patch
    {
        public static void Postfix(GraffitiGame __instance, GraffitiGame.GraffitiGameState setState)
        {
            if (Traverse.Create(__instance).Field<GraffitiSpot>("gSpot").Value is GraffitiSpotFinisher) return;
            if (Core.Instance.SaveManager.DataExists() && Core.Instance.Data.limitedGraffiti)
            {
                Traverse traverse = Traverse.Create(__instance);
                GraffitiSpot gSpot = traverse.Field<GraffitiSpot>("gSpot").Value;

                if (gSpot.bottomCrew != gSpot.topCrew && (gSpot.topCrew == Crew.PLAYERS || gSpot.bottomCrew == Crew.ROGUE))
                {
                    if (setState == GraffitiGame.GraffitiGameState.SHOW_PIECE && gSpot.attachedTo == GraffitiSpot.AttachType.DEFAULT)
                    {
                        GameplayUI ui = traverse.Field("player").Field<GameplayUI>("ui").Value;

                        string id = "";
                        if (gSpot.size == GraffitiSize.S)
                        {
                            Characters currentCharacter = Core.Instance.SaveManager.CurrentSaveSlot.currentCharacter;
                            Requirements.OverrideCharacterIfInvalid(ref currentCharacter);
                            id = currentCharacter.ToString();
                        }
                        else
                        {
                            id = traverse.Field<GraffitiArt>("grafArt").Value.unlockable.Uid;
                        }

                        int limit = Requirements.grafSLimit;
                        if (gSpot.size == GraffitiSize.M) limit = Requirements.grafMLimit;
                        else if (gSpot.size == GraffitiSize.L) limit = Requirements.grafLLimit;
                        else if (gSpot.size == GraffitiSize.XL) limit = Requirements.grafXLLimit;

                        if (Core.Instance.Data.grafUses.ContainsKey(id))
                        {
                            ui.graffitiTitle.text = $"{ui.graffitiTitle.text} ({limit - 1 - Core.Instance.Data.grafUses[id]} left)";
                        }
                    }
                }
            }
        }
    }
}
