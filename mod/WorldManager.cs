using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.Events;
using Reptile.Phone;
using UnityEngine;

namespace Archipelago
{
    public class WorldManager
    {
        public GameObject barricadeChunks;
        public GameObject progressObjectBel;
        public GameObject npcBel;

        public void LockDefaultGraffiti(HashSet<string> list)
        {
            foreach (string title in list)
            {
                GraffitiAppEntry graffiti = WorldHandler.instance.graffitiArtInfo.FindByTitle(title).unlockable;
                graffiti.IsDefault = false;
                Traverse.Create(Reptile.Core.Instance.BaseModule).Field<AUser>("user").Value.GetUnlockableSaveDataFor(graffiti).IsUnlocked = false;
            }
        }

        public void UnlockMaps(SaveSlotData save)
        {
            foreach (StageProgress progress in save.GetAllStageProgress())
            {
                progress.mapFound = true;
            }
        }

        public void SetSkateboardGarage(bool open)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.hideout)
            {
                Core.Logger.LogWarning("Current stage is not hideout, cannot set skateboard garage state.");
                return;
            }

            foreach (ProgressObject obj in Traverse.Create(WorldHandler.instance.StoryManager).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "GarageDoorSBClosed") obj.gameObject.SetActive(!open);
                if (obj.name == "GarageDoorSBOpen") obj.gameObject.SetActive(open);
            }
        }

        public void SetInlineGarage(bool open)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.hideout)
            {
                Core.Logger.LogWarning("Current stage is not hideout, cannot set inline garage state.");
                return;
            }

            foreach (ProgressObject obj in Traverse.Create(WorldHandler.instance.StoryManager).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "GarageDoorInlineClosed") obj.gameObject.SetActive(!open);
                if (obj.name == "GarageDoorInlineOpen") obj.gameObject.SetActive(open);
            }
        }

        public void SetBMXGarage(bool open)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.hideout)
            {
                Core.Logger.LogWarning("Current stage is not hideout, cannot set BMX garage state.");
                return;
            }

            foreach (ProgressObject obj in Traverse.Create(WorldHandler.instance.StoryManager).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "GarageDoorBMXClosed") obj.gameObject.SetActive(!open);
                if (obj.name == "GarageDoorBMXOpen") obj.gameObject.SetActive(open);
            }
        }

        public void SetGarages()
        {
            if (!Core.Instance.SaveManager.DataExists()) return;

            SetSkateboardGarage(Core.Instance.Data.skateboardUnlocked);
            SetInlineGarage(Core.Instance.Data.inlineUnlocked);
            SetBMXGarage(Core.Instance.Data.bmxUnlocked);
        }

        public int? GetNewRepRequirement(Stage stage, NPC npc)
        {
            switch (stage)
            {
                case Stage.downhill:
                    switch (npc.transform.parent.name)
                    {
                        case "WallrunsChallenge":
                            return 50;
                        case "ManualChallenge":
                            return 58;
                        case "CornersScoreChallenge":
                            return 65;
                        case "Crew_Wall":
                            return 90;
                        case "OldHeadWall":
                            return 120;
                        default:
                            return null;
                    }
                case Stage.square:
                    switch (npc.transform.name)
                    {
                        case "NPC_EclipseCrew":
                            return 180;
                        case "NPC_oldhead":
                            return 380;
                        default:
                            return null;
                    }
                case Stage.tower:
                    switch (npc.transform.parent.name)
                    {
                        case "UpsideGrind_Challenge":
                            return 188;
                        case "Manual_Challenge ":
                            return 200;
                        case "Score_Challenge":
                            return 220;
                        case "Chapter2":
                            switch (npc.transform.name)
                            {
                                case "CombatEncounter_SniperCaptain":
                                    return 260;
                                case "NPC_TowerGuard":
                                    return 280;
                                default:
                                    return null;
                            }
                        case "OldHeadWall":
                            switch (npc.requirement)
                            {
                                case 70:
                                    return 250;
                                case 140:
                                    return 320;
                                default:
                                    return null;
                            }
                        default:
                            return null;
                    }
                case Stage.Mall:
                    switch (npc.transform.parent.name)
                    {
                        case "Palms_Challenge":
                            return 434;
                        case "Slidepads_Challenge":
                            return 442;
                        case "Fish_Challenge ":
                            return 450;
                        case "Tricks_Challenge":
                            return 458;
                        case "Chapter3":
                            if (npc.transform.name == "NPC_Crew_GUARDING") return 491;
                            else return null;
                        case "Oldhead1":
                            return 530;
                        case "Oldhead2":
                            return 580;
                        default:
                            return null;
                    }
                case Stage.pyramid:
                    switch (npc.transform.parent.name)
                    {
                        case "Chapter4":
                            if (npc.transform.name == "NPC_OnDoor") return 620;
                            else if (npc.transform.name == "NPC_Crew_CrewBattle_Starter") return 730;
                            else return null;
                        case "Tricks_ScoreChallenge":
                            return 630;
                        case "RaceChallenge":
                            return 640;
                        case "Tricks2_ScoreChallenge":
                            return 650;
                        case "Manual_Challenge":
                            return 660;
                        case "OldHeadWall":
                            return 780;
                        default:
                            return null;
                    }
                case Stage.osaka:
                    switch (npc.transform.parent.name)
                    {
                        case "Chapter5":
                            if (npc.transform.name == "NPC_Crew_GateGuard") return 850;
                            else return null;
                        case "Race_Challenge":
                            return 864;
                        case "WallrunsChallenge":
                            return 880;
                        case "ScoreChallenge":
                            return 920;
                        case "OldHeadWall":
                        case "OldHeadWall2":
                            return 935;
                        case "CrewBattle":
                            return 960;
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        public void SetNPCRep(StoryManager sm)
        {
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc != null && npc.requirement != 0)
                {
                    if (GetNewRepRequirement(Reptile.Core.Instance.BaseModule.CurrentStage, npc).HasValue)
                    {
                        Core.Logger.LogInfo($"{npc.name} | {npc.transform.parent.name} | {npc.requirement}");
                        npc.requirement = GetNewRepRequirement(Reptile.Core.Instance.BaseModule.CurrentStage, npc).Value;
                    }
                }
            }
        }

        public void SetChapter4Character(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage == Stage.hideout) return;
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "ProgressObject_Exposition_HeadInsideHead")
                {
                    foreach (PersistentCall call in obj.OnExitSequence.m_PersistentCalls.m_Calls)
                    {
                        if (call.methodName == "SetPlayerAsCharacter")
                        {
                            Characters chara = Core.Instance.Data.firstCharacter;
                            if (chara == Characters.NONE) chara = Characters.metalHead;
                            call.arguments.stringArgument = chara.ToString();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void FindChapter1Objects(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.downhill) return;
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "ProgressObject_Bel")
                {
                    progressObjectBel = obj.gameObject;
                    npcBel = obj.gameObject.transform.parent.Find("NPC_Bel").gameObject;
                }
                if (obj.name == "BarricadeChunks1") barricadeChunks = obj.gameObject;
            }

            if (!Core.Instance.Data.hasM) {
                progressObjectBel.SetActive(false);
                npcBel.SetActive(false);
                barricadeChunks.SetActive(false);
            }
        }

        public void DeactivateChapter1Objects()
        {
            if (barricadeChunks == null) return;
            barricadeChunks.SetActive(false);

            if (Core.Instance.Data.hasM) return;
            progressObjectBel.SetActive(false);
            npcBel.SetActive(false);

        }

        public void ActivateChapter1Objects()
        {
            if (barricadeChunks == null) return;

            progressObjectBel.SetActive(true);
            npcBel.SetActive(true);
        }

        public void AddCallToPrinceCutscene(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.downhill) return;
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "PrinceIntroTrigger")
                {
                    obj.OnExitSequence.AddListener(delegate { DeactivateChapter1Objects(); });
                    break;
                }
            }
        }

        public void AddCallToFinalBoss(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj.name == "SnakeBossEncounter")
                {
                    ((CombatEncounter)obj).OnCompleted.AddListener(delegate { Core.Instance.Multiworld.SendCompletion(); });
                }
            }
        }

        public void DoStageSetup()
        {
            Stage stage = Reptile.Core.Instance.BaseModule.CurrentStage;
            Player player = WorldHandler.instance.GetCurrentPlayer();

            Traverse.Create(player).Field<float>("rep").Value = 0;
            Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = 0;

            List<Stage> changeRepStages = new List<Stage>()
            {
                Stage.downhill,
                Stage.square,
                Stage.tower,
                Stage.Mall,
                Stage.pyramid,
                Stage.osaka
            };

            if (changeRepStages.Contains(stage)) SetNPCRep(Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value);

            if (stage == Stage.hideout) SetChapter4Character(Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value);
            if (stage == Stage.downhill)
            {
                FindChapter1Objects(Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value);
                AddCallToPrinceCutscene(Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value);
            }
            if (stage == Stage.osaka) AddCallToFinalBoss(Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value);

            Traverse.Create(player).Field<float>("rep").Value = Core.Instance.Data.fakeRep;
            Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = Core.Instance.Data.fakeRep;
            Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1792;
        }
    }
}
