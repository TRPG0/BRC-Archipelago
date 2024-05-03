using Archipelago.BRC.Components;
using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.BRC.Stages
{
    public class OsakaManager : StageManager
    {
        public override bool HasChapter6Content => false;
        public override string StoryPath => "storyOsaka";

        public DreamEncounter dream;
        public NPC lion;
        public ProgressObject intro;


        public override void FindStoryObjects(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (NPC npc in sceneObjectsRegister.NPCs)
            {
                if (npc.name == "NPC_Crew_BattleStarter")
                {
                    CreateRequirementGraffiti(npc.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M, GraffitiSize.XL });
                    Core.Logger.LogInfo("Found NPC_Crew_BattleStarter");
                }
                else if (npc.name == "NPC_Lion")
                {
                    lion = npc;

                    if (Traverse.Create(npc).Field<int>("dialogueLevel").Value >= 8) UnlockChapter6Content();
                    else
                    {
                        npc.dialogues[7].OnEndSequence.AddListener(UnlockChapter6Content);
                        Core.Logger.LogInfo($"Added UnlockChapter6Content to OnEndSequence of dialogues[7] {npc.name}");
                    }
                }
            }
            foreach (ProgressObject po in sceneObjectsRegister.progressObjects)
            {
                if (po.name == "ProgressObject_ShowPathToFaux")
                {
                    Core.Logger.LogInfo("Found ProgressObject_ShowPathToFaux");
                    if (Core.Instance.Data.endingRep) CreateRequirementBoth(po.GetComponentInChildren<BoxCollider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M }, 1000, RequirementLinkType.ProgressObject);
                    else CreateRequirementGraffiti(po.GetComponentInChildren<BoxCollider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M }, RequirementLinkType.ProgressObject);
                }
                else if (po.name == "IntroSnakeBoss")
                {
                    Core.Logger.LogInfo("Found IntroSnakeBoss");
                    intro = po;
                }
            }
            foreach (GameplayEvent obj in sceneObjectsRegister.gameplayEvents)
            {
                if (obj.name == "TankwalkerCombatEncounter" && Core.Instance.Data.skipHands)
                {
                    ((CombatEncounter)obj).OnCompleted.AddListener(delegate 
                    { 
                        Traverse.Create(lion).Field<int>("dialogueLevel").Value = 8;
                        for (int i = 0; i < 8; i++)
                        {
                            Core.Instance.LocationManager.CountAndCheckSpray();
                        }
                    });
                    Core.Logger.LogInfo($"Added dialogue level change to OnCompleted of TankwalkerCombatEncounter");
                }
                else if (obj.name == "SnakeBossEncounter")
                {
                    ((CombatEncounter)obj).OnOutro.AddListener(Core.Instance.Multiworld.SendCompletion);
                    Core.Logger.LogInfo("Added SendCompletion call to SnakeBossEncounter");
                }
                else if (obj is DreamEncounter de)
                {
                    dream = de;
                    Core.Logger.LogInfo("Found DreamEncounter");
                }
            }

            if (!Traverse.Create(dream).Field<bool>("win").Value) DeactivateIntro();
        }

        public override void SkipDream(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (GameplayEvent obj in sceneObjectsRegister.gameplayEvents)
            {
                if (obj is DreamEncounter de)
                {
                    de.OnCompleted.AddListener(ActivateIntro);
                    Traverse traverse = Traverse.Create(de);
                    traverse.Field<bool>("startMusicAfterFirstCheckpoint").Value = false;
                    de.OnIntro.AddListener(delegate
                    {
                        if (Core.Instance.Data.skipDreams)
                        {
                            WorldHandler.instance.PlaceCurrentPlayerAt(de.checkpoints[5].spawnLocation);
                            WorldHandler.instance.GetCurrentPlayer().SetCharacter(Characters.legendFace);
                        }
                    });
                    Core.Logger.LogInfo($"Modified OnIntro of {de.name}");
                    return;
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

        public void DeactivateIntro()
        {
            if (intro == null) return;
            intro.gameObject.SetActive(false);
        }

        public void ActivateIntro()
        {
            if (intro == null) return;
            intro.gameObject.SetActive(true);
        }
    }
}
