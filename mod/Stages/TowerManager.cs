using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.BRC.Stages
{
    public class TowerManager : StageManager
    {
        public override string StoryPath => "Story_Tower";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_3;

        public override void FindStoryObjects(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (NPC npc in sceneObjectsRegister.NPCs)
            {
                if (npc.name == "NPC_Crew_Tower")
                {
                    CreateRequirementGraffiti(npc.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.L });
                    Core.Logger.LogInfo("Found NPC_Crew_Tower");
                    break;
                }
            }
        }
    }
}
