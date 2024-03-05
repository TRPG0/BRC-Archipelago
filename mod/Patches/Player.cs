using Archipelago.Components;
using Archipelago.Structures;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(Player), "ChangeHP")]
    public class Player_ChangeHP_Patch
    {
        public static void Prefix(ref int dmg)
        {
            if (Core.Instance.SaveManager.DataExists() && dmg > 0)
            {
                dmg *= Core.Instance.Data.damageMultiplier;
            }
        }
    }

    [HarmonyPatch(typeof(Player), "StartGraffitiMode")]
    public class Player_StartGraffitiMode_Patch
    {
        public static bool Prefix(GraffitiSpot graffitiSpot)
        {
            if (Core.Instance.Data.limitedGraffiti)
            {
                Encounter currentEncounter = Traverse.Create(WorldHandler.instance).Field<Encounter>("currentEncounter").Value;
                if (currentEncounter != null)
                {
                    if (currentEncounter.name.Contains("CrewBattle"))
                    {
                        Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                        return false;
                    }
                }
                if (graffitiSpot.size == GraffitiSize.S)
                {
                    if (Core.Instance.Data.sGraffiti == SGraffiti.Separate)
                    {
                        Characters currentCharacter = Core.Instance.SaveManager.CurrentSaveSlot.currentCharacter;
                        Requirements.OverrideCharacterIfInvalid(ref currentCharacter);
                        if (Core.Instance.Data.grafUses[currentCharacter.ToString()] >= Requirements.grafSLimit)
                        {
                            Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                            return false;
                        }
                    }
                    else
                    {
                        if (Core.Instance.Data.grafUses["S"] >= Core.Instance.Data.sMax)
                        {
                            Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                            return false;
                        }
                    }
                }
                return true;
            }
            else return true;
        }
    }

    [HarmonyPatch(typeof(Player), "OnTriggerEnter")]
    public class Player_OnTriggerEnter_Patch
    {
        public static bool Prefix(Collider other, Player __instance)
        {
            if (other.gameObject.layer == Layers.TriggerDetectPlayer && other.gameObject.name == "RandoRequirementTrigger" && !Traverse.Create(__instance).Field<bool>("isAI").Value)
            {
                //Core.Logger.LogInfo("Entered NeedGraffiti trigger");
                other.GetComponent<RandoRequirement>().ShowContext();
                return false;
            }
            else return true;
        }
    }

    [HarmonyPatch(typeof(Player), "OnTriggerExit")]
    public class Player_OnTriggerExit_Patch
    {
        public static bool Prefix(Collider other, Player __instance)
        {
            if (other.gameObject.layer == Layers.TriggerDetectPlayer && other.gameObject.name == "RandoRequirementTrigger" && !Traverse.Create(__instance).Field<bool>("isAI").Value)
            {
                //Core.Logger.LogInfo("Exited NeedGraffiti trigger");
                other.GetComponent<RandoRequirement>().HideContext();
                return false;
            }
            else return true;
        }
    }

    [HarmonyPatch(typeof(Player), "CheckNPCTriggerForConversation")]
    public class Player_CheckNPCTriggerForConversation_Patch
    {
        public static bool Prefix(NPC npc, ref bool __result)
        {
            if (Core.Instance.stageManager != null)
            {
                if (Core.Instance.stageManager.contextLabel.IsActive())
                {
                    __result = false;
                    return false;
                }
                else return true;
            }
            else return true;
        }
    }
}
