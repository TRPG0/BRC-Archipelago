using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago
{
    public class OsakaManager : StageManager
    {
        public GameObject osakaCrewBattleTrigger;
        public ProgressObject osakaSnakeTrigger;

        public void FindOsakaCrewBattle(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_BattleStarter")
                {
                    osakaCrewBattleTrigger = npc.transform.Find("Trigger").gameObject;
                    Core.Logger.LogInfo("Found NPC_Crew_BattleStarter");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M) || !Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.XL))
            {
                Core.Logger.LogInfo("Deactivated osaka crew battle trigger");
                osakaCrewBattleTrigger.SetActive(false);
            }
        }

        public void ActivateOsakaCrewBattle()
        {
            if (osakaCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Activated osaka crew battle trigger");
            osakaCrewBattleTrigger.SetActive(true);
        }

        public void DeactivateOsakaCrewBattle()
        {
            if (osakaCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Deactivated osaka crew battle trigger");
            osakaCrewBattleTrigger.SetActive(false);
        }

        public void FindOsakaSnakeTrigger(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (ProgressObject po in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (po.name == "ProgressObject_ShowPathToFaux")
                {
                    osakaSnakeTrigger = po;
                    Core.Logger.LogInfo("Found ProgressObject_ShowPathToFaux");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
            {
                Core.Logger.LogInfo("Deactivated osaka snake trigger");
                osakaSnakeTrigger.SetTriggerable(false);
            }
        }

        public void ActivateOsakaSnakeTrigger()
        {
            if (osakaSnakeTrigger == null) return;
            Core.Logger.LogInfo("Activated osaka snake battle trigger");
            osakaSnakeTrigger.SetTriggerable(true);
        }

        public void DeactivateOsakaSnakeTrigger()
        {
            if (osakaSnakeTrigger == null) return;
            Core.Logger.LogInfo("Deactivated osaka snake battle trigger");
            osakaSnakeTrigger.SetTriggerable(false);
        }

        public void AddCallToFinalBoss(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj.name == "SnakeBossEncounter")
                {
                    ((CombatEncounter)obj).OnCompleted.AddListener(delegate { Core.Instance.Multiworld.SendCompletion(); });
                    Core.Logger.LogInfo("Added SendCompletion call to SnakeBossEncounter");
                    break;
                }
            }
        }

        public override void SkipDream(StoryManager sm)
        {
            if (!Core.Instance.Data.skipDreams) return;
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj is DreamEncounter de)
                {
                    Traverse traverse = Traverse.Create(de);
                    traverse.Field<bool>("startMusicAfterFirstCheckpoint").Value = false;
                    de.OnIntro.AddListener(delegate
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(de.checkpoints[5].spawnLocation);
                        traverse.Method("SetPlayerAsCharacter", new object[] { Characters.legendFace }).GetValue();
                    });
                    Core.Logger.LogInfo($"Added PlaceCurrentPlayerAt to OnIntro of {de.name}");
                    break;
                }
            }
        }

        public override void DoStageSetup()
        {
            base.DoStageSetup();

            StoryManager sm = WorldHandler.instance.StoryManager;
            FindOsakaCrewBattle(sm);
            FindOsakaSnakeTrigger(sm);
            AddCallToFinalBoss(sm);
            SkipDream(sm);
        }

        public override void NoGraffitiM()
        {
            DeactivateOsakaCrewBattle();
            DeactivateOsakaSnakeTrigger();
        }

        public override void YesGraffitiM()
        {
            ActivateOsakaSnakeTrigger();
            if (Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.XL)) ActivateOsakaCrewBattle(); 
        }

        public override void NoGraffitiXL()
        {
            DeactivateOsakaCrewBattle();
        }

        public override void YesGraffitiXL()
        {
            if (Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M)) ActivateOsakaCrewBattle();
        }
    }
}
