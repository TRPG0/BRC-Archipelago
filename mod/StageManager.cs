using HarmonyLib;
using Reptile.Phone;
using Reptile;
using System.Collections.Generic;
using Archipelago.Components;
using Archipelago.Structures;
using Rewired;

namespace Archipelago
{
    public class StageManager
    {
        public PlayerChecker checker;

        public virtual bool ChangeNPCs => true;
        public virtual bool ChangeScores => true;

        public void SetupChecker()
        {
            checker = WorldHandler.instance.GetCurrentPlayer().gameObject.AddComponent<PlayerChecker>();
            checker.Init();
        }

        public void SetPlayerRep()
        {
            Traverse.Create(WorldHandler.instance.GetCurrentPlayer()).Field<float>("rep").Value = Core.Instance.Data.fakeRep;
            Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = Core.Instance.Data.fakeRep;

            if (Core.Instance.Data.totalRep == TotalRep.Normal) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1792;
            else if (Core.Instance.Data.totalRep == TotalRep.Less) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1472;
            else if (Core.Instance.Data.totalRep == TotalRep.MuchLess) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1184;
        }

        public void LockDefaultGraffiti(HashSet<string> list)
        {
            foreach (string title in list)
            {
                GraffitiAppEntry graffiti = WorldHandler.instance.graffitiArtInfo.FindByTitle(title).unlockable;
                graffiti.IsDefault = false;
                Traverse.Create(Reptile.Core.Instance.BaseModule).Field<AUser>("user").Value.GetUnlockableSaveDataFor(graffiti).IsUnlocked = false;
            }
        }

        public void SetNPCRep(StoryManager sm)
        {
            foreach (NPC npc in Traverse.Create(sm).Field<List<NPC>>("npcs").Value)
            {
                if (npc != null && npc.requirement != 0)
                {
                    if (Requirements.GetNPCNewRep(Reptile.Core.Instance.BaseModule.CurrentStage, npc).HasValue)
                    {
                        Core.Logger.LogInfo($"{npc.name} | {npc.transform.parent.name} | {npc.requirement} > {Requirements.GetNPCNewRep(Reptile.Core.Instance.BaseModule.CurrentStage, npc).Value}");
                        npc.requirement = Requirements.GetNPCNewRep(Reptile.Core.Instance.BaseModule.CurrentStage, npc).Value;
                    }
                }
            }
        }

        public void SetEncounterScores(StoryManager sm)
        {
            foreach (GameplayEvent ge in Traverse.Create(sm).Field<List<GameplayEvent>>("gameplayEvents").Value)
            {
                if (ge is ScoreEncounter se)
                {
                    ScoreValues values = Requirements.GetEncounterNewScore(Reptile.Core.Instance.BaseModule.CurrentStage, se);
                    if (values == null) continue;

                    int target = values.oldValue;
                    if (Core.Instance.Data.scoreDifficulty == ScoreDifficulty.Hard) target = values.hardValue;
                    if (Core.Instance.Data.scoreDifficulty == ScoreDifficulty.VeryHard) target = values.veryHardValue;
                    if (Core.Instance.Data.scoreDifficulty == ScoreDifficulty.Extreme) target = values.extremeValue;

                    se.targetScore = target;
                    Core.Logger.LogInfo($"Setting {se.name} target score to {target} (original: {values.oldValue})");
                }
            }
        }

        public virtual void SkipDream(StoryManager sm)
        {
            if (!Core.Instance.Data.skipDreams) return;
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

        public virtual void DoStageSetup() 
        {
            SetupChecker();
            StoryManager sm = WorldHandler.instance.StoryManager;
            if (ChangeNPCs) SetNPCRep(sm);
            if (ChangeScores && Core.Instance.Data.scoreDifficulty > ScoreDifficulty.Normal) SetEncounterScores(sm);
            SetPlayerRep();
            LockDefaultGraffiti(Core.Instance.Data.to_lock);
            Core.Instance.PhoneManager.DoAppSetup();
        }

        public virtual void NoGraffitiM() { }
        public virtual void YesGraffitiM() { }
        public virtual void NoGraffitiL() { }
        public virtual void YesGraffitiL() { }
        public virtual void NoGraffitiXL() { }
        public virtual void YesGraffitiXL() { }
    }
}
