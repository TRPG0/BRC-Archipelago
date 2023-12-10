using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.Stages
{
    public class TowerManager : StageManager
    {
        public override string StoryPath => "Story_Tower";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_3;

        public override void FindStoryObjects(StoryManager sm)
        {
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_Tower")
                {
                    CreateNeedGraffitiCollider(npc.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.L });
                    Core.Logger.LogInfo("Found NPC_Crew_Tower");
                    break;
                }
            }
        }
    }
}
