using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago
{
    public class MallManager : StageManager
    {
        public NPC mallPoliceNPC;
        public GameObject mallCrewBattleTrigger; 

        public void FindMallNPCs(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.Mall) return;
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_policeActivator")
                {
                    mallPoliceNPC = npc;
                    Core.Logger.LogInfo("Found NPC_policeActivator");
                }
                else if (npc.name == "NPC_Crew_BattleStarter")
                {
                    mallCrewBattleTrigger = npc.transform.Find("Sphere (1)").gameObject;
                    Core.Logger.LogInfo("Found NPC_Crew_BattleStarter");
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
            {
                Core.Logger.LogInfo("Set police activator requirement to 5");
                mallPoliceNPC.requirement = 5;
            }
            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.L))
            {
                Core.Logger.LogInfo("Deactivated mall crew battle trigger");
                mallCrewBattleTrigger.SetActive(false);
            }
        }

        public void ActivateMallPoliceNPC()
        {
            if (mallPoliceNPC == null) return;
            Core.Logger.LogInfo("Set police activator requirement to 4");
            mallPoliceNPC.requirement = 4;
        }

        public void DeactivateMallPoliceNPC()
        {
            if (mallPoliceNPC == null) return;
            Core.Logger.LogInfo("Set police activator requirement to 5");
            mallPoliceNPC.requirement = 5;
        }

        public void ActivateMallCrewBattle()
        {
            if (mallCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Activated mall crew battle trigger");
            mallCrewBattleTrigger.SetActive(true);
        }

        public void DeactivateMallCrewBattle()
        {
            if (mallCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Deactivated mall crew battle trigger");
            mallCrewBattleTrigger.SetActive(false);
        }

        public override void DoStageSetup()
        {
            base.DoStageSetup();

            StoryManager sm = WorldHandler.instance.StoryManager;
            FindMallNPCs(sm);
            SkipDream(sm);
        }

        public override void NoGraffitiM()
        {
            DeactivateMallPoliceNPC();
        }

        public override void YesGraffitiM()
        {
            ActivateMallPoliceNPC();
        }

        public override void NoGraffitiL()
        {
            DeactivateMallCrewBattle();
        }

        public override void YesGraffitiL()
        {
            ActivateMallCrewBattle();
        }
    }
}
