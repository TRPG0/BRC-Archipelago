using HarmonyLib;
using Reptile.Phone;
using Reptile;
using System.Collections.Generic;
using Archipelago.Components;
using Rewired;

namespace Archipelago
{
    public class StageManager
    {
        public PlayerChecker checker;

        public virtual bool ChangeNPCs => true;

        public void SetupChecker()
        {
            checker = WorldHandler.instance.GetCurrentPlayer().gameObject.AddComponent<PlayerChecker>();
            checker.Init();
        }

        public void SetPlayerRep()
        {
            Traverse.Create(WorldHandler.instance.GetCurrentPlayer()).Field<float>("rep").Value = Core.Instance.Data.fakeRep;
            Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = Core.Instance.Data.fakeRep;

            if (Core.Instance.Data.totalRep == 0) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1792;
            else if (Core.Instance.Data.totalRep == 1) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1472;
            else if (Core.Instance.Data.totalRep == 2) Traverse.Create(WorldHandler.instance).Field<int>("totalREPInCurrentStage").Value = 1184;
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

        public void SetCrewBattleScore(StoryManager sm, int score)
        {
            if (!Core.Instance.Data.hardBattles) return;
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

        public virtual void DoStageSetup() 
        {
            SetupChecker();
            if (ChangeNPCs) SetNPCRep(Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value);
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
