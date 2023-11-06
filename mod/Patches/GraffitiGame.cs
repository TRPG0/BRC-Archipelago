using HarmonyLib;
using Reptile;
using System.Collections.Generic;

namespace Archipelago.Patches
{
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
                    if (gSpot.size != GraffitiSize.S && gSpot.attachedTo == GraffitiSpot.AttachType.DEFAULT)
                    {
                        if (gSpot.bottomCrew != gSpot.topCrew && (gSpot.topCrew == Crew.PLAYERS || gSpot.bottomCrew == Crew.ROGUE) && Reptile.Core.Instance.BaseModule.CurrentStage != Stage.Prelude)
                        {
                            GraffitiArt grafArt = Traverse.Create(__instance).Field<GraffitiArt>("grafArt").Value;

                            int limit = 10;
                            if (gSpot.size == GraffitiSize.XL) limit = 6;

                            Core.Instance.Data.grafUses[grafArt.unlockable.Uid] += 1;
                            if (Core.Instance.Data.grafUses[grafArt.unlockable.Uid] >= limit)
                            {
                                grafArt.unlockable.IsDefault = false;
                                Core.Instance.SaveManager.CurrentSaveSlot.GetUnlockableDataByUid(grafArt.unlockable.Uid).IsUnlocked = false;
                                Core.Instance.LocationManager.notifQueue.Add(new Structures.Notification("AppGraffiti", $"\"{grafArt.title}\" has been depleted.", null));

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

                                if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
                                {
                                    Core.Instance.stageManager.NoGraffitiM();
                                }
                                if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.L))
                                {
                                    Core.Instance.stageManager.NoGraffitiL();
                                }
                                if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.XL))
                                {
                                    Core.Instance.stageManager.NoGraffitiXL();
                                }
                            }

                            Core.Instance.SaveManager.SaveData();
                            string usage = $"\"{grafArt.title}\" has been used {Core.Instance.Data.grafUses[grafArt.unlockable.Uid]}";
                            if (Core.Instance.Data.grafUses[grafArt.unlockable.Uid] == 1) usage += " time.";
                            else usage += " times.";
                            Core.Logger.LogInfo(usage);
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
                    if (setState == GraffitiGame.GraffitiGameState.SHOW_PIECE && gSpot.size != GraffitiSize.S && gSpot.attachedTo == GraffitiSpot.AttachType.DEFAULT)
                    {
                        GraffitiArt grafArt = traverse.Field<GraffitiArt>("grafArt").Value;
                        GameplayUI ui = traverse.Field("player").Field<GameplayUI>("ui").Value;

                        int limit = 10;
                        if (traverse.Field<GraffitiSpot>("gSpot").Value.size == GraffitiSize.XL) limit = 6;


                        if (Core.Instance.Data.grafUses.ContainsKey(grafArt.unlockable.Uid))
                        {
                            ui.graffitiTitle.text = $"{ui.graffitiTitle.text} ({limit - 1 - Core.Instance.Data.grafUses[grafArt.unlockable.Uid]} left)";
                        }
                    }
                }
            }
        }
    }
}
