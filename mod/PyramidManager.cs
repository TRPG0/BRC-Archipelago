using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archipelago
{
    public class PyramidManager : StageManager
    {
        public GameObject pyramidCopterTrigger;
        public GameObject pyramidDJTrigger;

        public void FindPyramidCopterEncounter(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.pyramid) return;
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_CopterEncounter_Starter")
                {
                    pyramidCopterTrigger = npc.transform.Find("Trigger").gameObject;
                    Core.Logger.LogInfo("Found NPC_Crew_CopterEncounter_Starter");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
            {
                Core.Logger.LogInfo("Deactivated helicopter encounter trigger");
                pyramidCopterTrigger.SetActive(false);
            }
        }

        public void ActivatePyramidCopterBattle()
        {
            if (pyramidCopterTrigger == null) return;
            Core.Logger.LogInfo("Activated helicopter encounter trigger");
            pyramidCopterTrigger.SetActive(true);
        }

        public void DeactivatePyramidCopterBattle()
        {
            if (pyramidCopterTrigger == null) return;
            Core.Logger.LogInfo("Deactivated helicopter encounter trigger");
            pyramidCopterTrigger.SetActive(false);
        }

        public void FindPyramidDJEncounter(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.pyramid) return;
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj.name == "DJBossEncounterPhase1")
                {
                    pyramidDJTrigger = obj.transform.Find("Trigger").gameObject;
                    Core.Logger.LogInfo("Found DJBossEncounterPhase1");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.L))
            {
                Core.Logger.LogInfo("Deactivated pyramid DJ battle trigger");
                pyramidDJTrigger.SetActive(false);
            }
        }

        public void ActivatePyramidDJBattle()
        {
            if (pyramidDJTrigger == null) return;
            Core.Logger.LogInfo("Activated pyramid DJ battle trigger");
            pyramidDJTrigger.SetActive(true);
        }

        public void DeactivatePyramidDJBattle()
        {
            if (pyramidDJTrigger == null) return;
            Core.Logger.LogInfo("Deactivated pyramid DJ battle trigger");
            pyramidDJTrigger.SetActive(false);
        }

        public override void DoStageSetup()
        {
            base.DoStageSetup();

            StoryManager sm = WorldHandler.instance.StoryManager;
            FindPyramidCopterEncounter(sm);
            FindPyramidDJEncounter(sm);
            SkipDream(sm);
        }

        public override void NoGraffitiM()
        {
            DeactivatePyramidCopterBattle();
        }

        public override void YesGraffitiM()
        {
            ActivatePyramidCopterBattle();
        }

        public override void NoGraffitiL()
        {
            DeactivatePyramidDJBattle();
        }

        public override void YesGraffitiL()
        {
            ActivatePyramidDJBattle();
        }
    }
}
