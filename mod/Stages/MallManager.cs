using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.Stages
{
    public class MallManager : StageManager
    {
        public override string StoryPath => "story_Mall";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_4;

        public NPC mallPoliceNPC;

        public override void FindStoryObjects(SceneObjectsRegister sceneObjectsRegister)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.Mall) return;
            foreach (NPC npc in sceneObjectsRegister.NPCs)
            {
                if (npc.name == "NPC_policeActivator")
                {
                    mallPoliceNPC = npc;
                    Core.Logger.LogInfo("Found NPC_policeActivator");
                }
                else if (npc.name == "NPC_Crew_BattleStarter")
                {
                    CreateNeedGraffitiCollider(npc.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.L });
                    Core.Logger.LogInfo("Found NPC_Crew_BattleStarter");
                }
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

        public override void NoGraffiti(GraffitiSize size)
        {
            base.NoGraffiti(size);

            if (size == GraffitiSize.M) DeactivateMallPoliceNPC();
        }

        public override void YesGraffiti(GraffitiSize size)
        {
            base.YesGraffiti(size);

            if (size == GraffitiSize.M) ActivateMallPoliceNPC();
        }
    }
}
