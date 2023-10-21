using Reptile;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.Events;
using Reptile.Phone;
using UnityEngine;
using Archipelago.Components;

namespace Archipelago
{
    public class WorldManager
    {
        public PlayerChecker checker;

        public GameObject barricadeChunks;
        public GameObject progressObjectBel;
        public GameObject npcBel;
        public GameObject princeCrewBattleTrigger;

        public GameObject towerCrewBattleTrigger;

        public NPC mallPoliceNPC;
        public GameObject mallCrewBattleTrigger;

        public GameObject pyramidCopterTrigger;
        public GameObject pyramidDJTrigger;

        public GameObject osakaCrewBattleTrigger;
        public ProgressObject osakaSnakeTrigger;

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

        public void SetNPCRep(StoryManager sm)
        {
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc != null && npc.requirement != 0)
                {
                    if (RepRequirements.GetNPCNewRep(Reptile.Core.Instance.BaseModule.CurrentStage, npc).HasValue)
                    {
                        Core.Logger.LogInfo($"{npc.name} | {npc.transform.parent.name} | {npc.requirement} > {RepRequirements.GetNPCNewRep(Reptile.Core.Instance.BaseModule.CurrentStage, npc).Value}");
                        npc.requirement = RepRequirements.GetNPCNewRep(Reptile.Core.Instance.BaseModule.CurrentStage, npc).Value;
                    }
                }
            }
        }

        public void SetChapter4Character(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.hideout) return;
            if (Core.Instance.Data.firstCharacter == Characters.blockGuy) return;
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "ProgressObject_Exposition_HeadInsideHead")
                {
                    Characters chara = Core.Instance.Data.firstCharacter;
                    if (chara == Characters.NONE) chara = Characters.metalHead;
                    obj.OnExitSequence.AddListener(delegate { obj.SetPlayerAsCharacter(chara.ToString()); });
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
                    Core.Logger.LogInfo("Found ProgressObject_Bel");
                    npcBel = obj.gameObject.transform.parent.Find("NPC_Bel").gameObject;
                    Core.Logger.LogInfo("Found NPC_Bel");
                }
                else if (obj.name == "BarricadeChunks1")
                {
                    barricadeChunks = obj.gameObject;
                    Core.Logger.LogInfo("Found BarricadeChunks1");
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
            {
                Core.Logger.LogInfo("Deactivated downhill story objects");
                progressObjectBel.SetActive(false);
                npcBel.SetActive(false);
                barricadeChunks.SetActive(false);
            }
        }

        public void DeactivateChapter1Objects()
        {
            if (barricadeChunks == null) return;
            Core.Logger.LogInfo("Deactivated downhill story objects");
            barricadeChunks.SetActive(false);

            if (Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M)) return;
            progressObjectBel.SetActive(false);
            npcBel.SetActive(false);

        }

        public void ActivateChapter1Objects()
        {
            if (barricadeChunks == null) return;
            Core.Logger.LogInfo("Activated downhill story objects");

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

        public void FindPrinceCrewBattleTrigger(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.downhill) return;
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "BackToPrinceTrigger")
                {
                    princeCrewBattleTrigger = obj.transform.Find("trigger").gameObject;
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
            {
                Core.Logger.LogInfo("Deactivated prince crew battle trigger");
                princeCrewBattleTrigger.SetActive(false);
            }
        }

        public void ActivatePrinceCrewBattleTrigger()
        {
            if (princeCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Activated prince crew battle trigger");
            princeCrewBattleTrigger.SetActive(true);
        }

        public void DeactivatePrinceCrewBattleTrigger()
        {
            if (princeCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Deactivated prince crew battle trigger");
            princeCrewBattleTrigger.SetActive(false);
        }

        public void DontUnlockCharacterSelect(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.square) return;
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "ProgressObject_Phonecall_TeachCypher")
                {
                    Traverse.Create(obj).Field<UnityEvent>("OnTrigger").Value.m_PersistentCalls.Clear();
                    Core.Logger.LogInfo("Removed character select unlock from call with Tryce");
                    break;
                }
            }
        }

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

        public void FindOsakaCrewBattle(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc.name == "NPC_Crew_BattleStarter")
                {
                    osakaCrewBattleTrigger = npc.transform.Find("Trigger").gameObject;
                    Core.Logger.LogInfo("Found NPC_Crew_BattleStarter");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.XL))
            {
                Core.Logger.LogInfo("Deactivated osaka crew battle trigger");
                osakaCrewBattleTrigger.SetActive(false);
            }
        }

        public void ActivateOsakaCrewBattle()
        {
            if (osakaCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Activated osaka crew battle trigger");
            osakaCrewBattleTrigger.SetActive(true);
        }

        public void DeactivateOsakaCrewBattle()
        {
            if (osakaCrewBattleTrigger == null) return;
            Core.Logger.LogInfo("Deactivated osaka crew battle trigger");
            osakaCrewBattleTrigger.SetActive(false);
        }

        public void FindOsakaSnakeTrigger(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (ProgressObject po in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (po.name == "ProgressObject_ShowPathToFaux")
                {
                    osakaSnakeTrigger = po;
                    Core.Logger.LogInfo("Found ProgressObject_ShowPathToFaux");
                    break;
                }
            }

            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M))
            {
                Core.Logger.LogInfo("Deactivated osaka snake trigger");
                osakaSnakeTrigger.SetTriggerable(false);
            }
        }

        public void ActivateOsakaSnakeTrigger()
        {
            if (osakaSnakeTrigger == null) return;
            Core.Logger.LogInfo("Activated osaka snake battle trigger");
            osakaSnakeTrigger.SetTriggerable(true);
        }

        public void DeactivateOsakaSnakeTrigger()
        {
            if (osakaSnakeTrigger == null) return;
            Core.Logger.LogInfo("Deactivated osaka snake battle trigger");
            osakaSnakeTrigger.SetTriggerable(false);
        }

        public void AddCallToFinalBoss(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
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

        public void SkipDreams(StoryManager sm)
        {
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj is DreamEncounter de)
                {
                    de.OnIntro.AddListener(delegate { de.CheatComplete(); });
                    Core.Logger.LogInfo($"Added CheatComplete to OnIntro of {de.name}");
                    break;
                }
            }
        }

        public void SkipDream5(StoryManager sm)
        {
            if (Reptile.Core.Instance.BaseModule.CurrentStage != Stage.osaka) return;
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj is DreamEncounter de)
                {
                    Traverse traverse = Traverse.Create(de);
                    traverse.Field<bool>("startMusicAfterFirstCheckpoint").Value = false;
                    de.OnIntro.AddListener(delegate 
                    {
                        WorldHandler.instance.PlaceCurrentPlayerAt(de.checkpoints[5].spawnLocation);
                        traverse.Method("SetPlayerAsCharacter", new object[] { Characters.legendFace }).GetValue();
                    });
                    Core.Logger.LogInfo($"Added PlaceCurrentPlayerAt to OnIntro of {de.name}");
                    break;
                }
            }
        }

        public void SetCrewBattleScore(StoryManager sm, int score)
        {
            foreach (GameplayEvent obj in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (obj is ScoreEncounter se && se.name.Contains("CrewBattle"))
                {
                    Core.Logger.LogInfo($"Setting {se.name} target score to {score} (prev: {se.targetScore})");
                    se.targetScore = score;
                    break;
                }
            }
        }

        public void DoStageSetup()
        {
            Stage stage = Reptile.Core.Instance.BaseModule.CurrentStage;
            Core.Logger.LogInfo($"Current stage is {stage}");
            StoryManager sm = Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value;
            Player player = WorldHandler.instance.GetCurrentPlayer();
            
            if (stage != Stage.Prelude)
            {
                checker = player.gameObject.AddComponent<PlayerChecker>();
                checker.Init();
            }

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

            Dictionary<Stage, int> crewScores = new Dictionary<Stage, int>()
            {
                [Stage.downhill] = 1500000, // 2 mil
                [Stage.tower] = 2500000, // 4 mil
                [Stage.Mall] = 2500000, // 3 mil?
                [Stage.pyramid] = 3000000,
                [Stage.osaka] = 4000000
            };

            if (changeRepStages.Contains(stage))
            {
                SetNPCRep(sm);
                if (Core.Instance.Data.skipDreams)
                {
                    if (stage == Stage.osaka) SkipDream5(sm);
                    else SkipDreams(sm);
                }
            }

            if (Core.Instance.Data.hardBattles && crewScores.ContainsKey(stage)) SetCrewBattleScore(sm, crewScores[stage]);

            if (stage == Stage.hideout) SetChapter4Character(sm);
            else if (stage == Stage.downhill)
            {
                FindChapter1Objects(sm);
                AddCallToPrinceCutscene(sm);
                FindPrinceCrewBattleTrigger(sm);
            }
            else if (stage == Stage.square)
            {
                DontUnlockCharacterSelect(sm);
            }
            else if (stage == Stage.tower)
            {
                FindTowerCrewBattle(sm);
            }
            else if (stage == Stage.Mall)
            {
                FindMallNPCs(sm);
            }
            else if (stage == Stage.pyramid)
            {
                FindPyramidCopterEncounter(sm);
                FindPyramidDJEncounter(sm);
            }
            else if (stage == Stage.osaka)
            {
                FindOsakaCrewBattle(sm);
                FindOsakaSnakeTrigger(sm);
                AddCallToFinalBoss(sm);
            }

            Traverse.Create(player).Field<float>("rep").Value = Core.Instance.Data.fakeRep;
            Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = Core.Instance.Data.fakeRep;

            if (Core.Instance.Data.totalRep == 0) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1792;
            else if (Core.Instance.Data.totalRep == 1) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1472;
            else if (Core.Instance.Data.totalRep == 2) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1184;
        }
    }
}
