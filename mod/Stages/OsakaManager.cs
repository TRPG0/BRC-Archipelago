using Archipelago.Components;
using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.Stages
{
    public class OsakaManager : StageManager
    {
        public override bool HasChapter6Content => false;
        public override string StoryPath => "storyOsaka";


        public override void FindStoryObjects(StoryManager sm)
        {
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_BattleStarter")
                {
                    CreateNeedGraffitiCollider(npc.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M, GraffitiSize.XL });
                    Core.Logger.LogInfo("Found NPC_Crew_BattleStarter");
                }
                else if (npc.name == "NPC_Lion")
                {
                    if (Traverse.Create(npc).Field<int>("dialogueLevel").Value >= 8) UnlockChapter6Content();
                    else
                    {
                        npc.dialogues[7].OnEndSequence.AddListener(delegate { UnlockChapter6Content(); });
                        Core.Logger.LogInfo($"Added UnlockChapter6Content to OnEndSequence of {npc.name}");
                    }
                }
            }
            foreach (ProgressObject po in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (po.name == "ProgressObject_ShowPathToFaux")
                {
                    Core.Logger.LogInfo("Found ProgressObject_ShowPathToFaux");
                    CreateNeedGraffitiCollider(po.GetComponentInChildren<BoxCollider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M }, NeedGraffiti.NeedGraffitiType.ProgressObject);
                    break;
                }
            }
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
                        WorldHandler.instance.GetCurrentPlayer().SetCharacter(Characters.legendFace);
                    });
                    Core.Logger.LogInfo($"Added PlaceCurrentPlayerAt to OnIntro of {de.name}");
                    break;
                }
            }
        }

        public override void UnlockChapter6Content()
        {
            ActiveOnChapter chapterComponent = GameObject.Find(StoryPath).transform.Find(Chapter6Object).GetComponent<ActiveOnChapter>();
            chapterComponent.chapters.Add(Story.Chapter.CHAPTER_5);
            chapterComponent.OnStageInitialized();

            GameObject parts = GameObject.Find("StagePartsThatShouldNotBeInChunksForCombiningReasons");
            parts.transform.Find("BeforeFinalBossElephants").gameObject.SetActive(false);
            parts.transform.Find("ProgressObject_AdditionalGameplayAfterFinalBoss").gameObject.SetActive(true);
            Core.Logger.LogInfo("Chapter 6 content unlocked.");
        }
    }
}
