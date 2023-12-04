﻿using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Archipelago.Stages
{
    public class PyramidManager : StageManager
    {
        public override string StoryPath => "story_Pyramid";
        public override string Chapter6Object => "Chapter 6";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_5;

        public override void FindStoryObjects(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.pyramid) return;
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_CopterEncounter_Starter")
                {
                    CreateNeedGraffitiCollider(npc.GetComponentInChildren<Collider>().gameObject, new List<GraffitiSize>() { GraffitiSize.M });
                    Core.Logger.LogInfo("Found NPC_Crew_CopterEncounter_Starter");
                    break;
                }
            }
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj.name == "DJBossEncounterPhase1")
                {
                    CreateNeedGraffitiCollider(obj.GetComponentInChildren<Collider>().gameObject, new List<GraffitiSize> { GraffitiSize.L });
                    Core.Logger.LogInfo("Found DJBossEncounterPhase1");
                    break;
                }
            }
        }
    }
}