using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago
{
    public class TowerManager : StageManager
    {
        public GameObject towerCrewBattleTrigger;

        public void FindTowerCrewBattle(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.tower) return;
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_Tower")
                {
                    towerCrewBattleTrigger = npc.transform.Find("Trigger").gameObject;
                    Core.Logger.LogInfo("Found NPC_Crew_Tower");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.L))
            {
                Core.Logger.LogInfo("Deactivated tower crew battle trigger");
                towerCrewBattleTrigger.SetActive(false);
            }
        }

        public void ActivateTowerCrewBattle()
        {
            if (towerCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Activated tower crew battle trigger");
            towerCrewBattleTrigger.SetActive(true);
        }

        public void DeactivateTowerCrewBattle()
        {
            if (towerCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Deactivated tower crew battle trigger");
            towerCrewBattleTrigger.SetActive(false);
        }

        public override void DoStageSetup()
        {
            base.DoStageSetup();

            StoryManager sm = Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value;
            FindTowerCrewBattle(sm);
            SetCrewBattleScore(sm, 2500000);
            SkipDream(sm);
        }

        public override void NoGraffitiL()
        {
            DeactivateTowerCrewBattle();
        }

        public override void YesGraffitiL()
        {
            ActivateTowerCrewBattle();
        }
    }
}
