using HarmonyLib;
using Reptile.Phone;
using Reptile;
using System.Collections.Generic;
using Archipelago.Components;
using Archipelago.Structures;
using Rewired;
using UnityEngine;
using TMPro;
using System;

namespace Archipelago.Stages
{
    public class StageManager
    {
        public PlayerChecker checker;

        public GameObject contextGraffitiIcon;
        public TextMeshProUGUI contextLabel;
        public TextMeshProUGUI dieMenuReasonLabel;

        public List<NeedGraffiti> graffitiColliders = new List<NeedGraffiti>();

        public virtual bool ChangeNPCs => true;
        public virtual bool ChangeScores => true;
        public virtual bool HasDream => true;

        public virtual bool HasChapter6Content => true;
        public virtual string StoryPath => "";
        public virtual string Chapter6Object => "Chapter6";
        public virtual Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_1;

        public void SetupChecker()
        {
            checker = WorldHandler.instance.GetCurrentPlayer().gameObject.AddComponent<PlayerChecker>();
            checker.Init();
        }

        public void SetupUI()
        {
            GameplayUI ui = Traverse.Create(WorldHandler.instance.GetCurrentPlayer()).Field<GameplayUI>("ui").Value;
            contextGraffitiIcon = GameObject.Instantiate(ui.contextGraffitiIcon.gameObject, ui.contextGraffitiIcon.transform.parent, false);
            contextLabel = GameObject.Instantiate(ui.contextLabelGameObject, ui.contextLabelGameObject.transform.parent, false).GetComponent<TextMeshProUGUI>();
            contextLabel.font = ui.repLabel.font;
            contextLabel.fontMaterial = ui.repLabel.materialForRendering;
            contextLabel.faceColor = Color.white;
            contextLabel.outlineColor = Color.black;
            contextLabel.fontStyle = FontStyles.UpperCase;
            Component.Destroy(contextLabel.gameObject.GetComponent<UIButtonGlyphComponent>());
            contextLabel.gameObject.AddComponent<OutlineSetter>().SetText(contextLabel);
            contextGraffitiIcon.SetActive(false);
            contextLabel.gameObject.SetActive(false);
            DieMenu dm = Traverse.Create(Reptile.Core.Instance.UIManager).Field<DieMenu>("dieMenu").Value;
            dieMenuReasonLabel = GameObject.Instantiate(dm.transform.Find("DieMenuTitleLabel"), dm.transform).GetComponent<TextMeshProUGUI>();
            Vector3 pos = dieMenuReasonLabel.transform.localPosition;
            pos.y = pos.y - 80;
            dieMenuReasonLabel.transform.localPosition = pos;
            dieMenuReasonLabel.fontSize = 50;
            dieMenuReasonLabel.color = new Color(1, 1, 1, 0);
            Component.Destroy(dieMenuReasonLabel.GetComponent<TMProFontLocalizer>());
            Component.Destroy(dieMenuReasonLabel.GetComponent<TMProLocalizationAddOn>());
            dieMenuReasonLabel.gameObject.AddComponent<DeathLinkText>().Init(dm.transform.Find("DieMenuTitleLabel").GetComponent<TextMeshProUGUI>());
            dieMenuReasonLabel.gameObject.SetActive(false);
        }

        public void CreateNeedGraffitiCollider(GameObject original, List<GraffitiSize> sizes, NeedGraffiti.NeedGraffitiType type = NeedGraffiti.NeedGraffitiType.Trigger)
        {
            GameObject clone = GameObject.Instantiate(original, original.transform.parent);
            clone.AddComponent<NeedGraffiti>().Init(sizes, original, type);
            graffitiColliders.Add(clone.GetComponent<NeedGraffiti>());
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
                Reptile.Core.Instance.Platform.User.GetUnlockableSaveDataFor(graffiti).IsUnlocked = false;
            }
        }

        public void SetNPCRep(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (NPC npc in sceneObjectsRegister.NPCs)
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

        public void SetEncounterScores(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (GameplayEvent ge in sceneObjectsRegister.gameplayEvents)
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

        public virtual void SkipDream(SceneObjectsRegister sceneObjectsRegister)
        {
            if (!Core.Instance.Data.skipDreams) return;
            foreach (GameplayEvent obj in sceneObjectsRegister.gameplayEvents)
            {
                if (obj is DreamEncounter de)
                {
                    de.OnIntro.AddListener(delegate { de.CheatComplete(); });
                    Core.Logger.LogInfo($"Added CheatComplete to OnIntro of {de.name}");
                    break;
                }
            }
        }

        public virtual void UnlockChapter6Content()
        {
            ActiveOnChapter chapterComponent = GameObject.Find(StoryPath).transform.Find(Chapter6Object).GetComponent<ActiveOnChapter>();
            List<Story.Chapter> chapters = new List<Story.Chapter>();
            if (MinimumChapter < Story.Chapter.CHAPTER_6)
            {
                for (int i = (int)MinimumChapter; i <= (int)Story.Chapter.CHAPTER_6; i++)
                {
                    chapters.Add((Story.Chapter)i);
                }
            }
            else chapters.Add(Story.Chapter.CHAPTER_6);
            chapterComponent.chapters = chapters;
            chapterComponent.OnStageInitialized();
        }

        public virtual void DoStageSetup()
        {
            SetupChecker();
            SetupUI();
            SceneObjectsRegister sor = WorldHandler.instance.SceneObjectsRegister;
            if (ChangeNPCs) SetNPCRep(sor);
            if (ChangeScores && Core.Instance.Data.scoreDifficulty > ScoreDifficulty.Normal) SetEncounterScores(sor);
            if (HasDream) SkipDream(sor);
            if (HasChapter6Content) UnlockChapter6Content();
            SetPlayerRep();
            LockDefaultGraffiti(Core.Instance.Data.to_lock);
            Core.Instance.PhoneManager.DoAppSetup();
            FindStoryObjects(sor);
            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.M)) NoGraffiti(GraffitiSize.M);
            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.L)) NoGraffiti(GraffitiSize.L);
            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(GraffitiSize.XL)) NoGraffiti(GraffitiSize.XL);
        }

        public virtual void NoGraffiti(GraffitiSize size) 
        {
            foreach (NeedGraffiti ngc in graffitiColliders)
            {
                if (ngc.Sizes.Contains(size))
                {
                    ngc.DisableTrigger();
                    Core.Logger.LogInfo($"Disabled trigger of object {ngc.transform.parent.name}");
                }
            }
        }

        public virtual void YesGraffiti(GraffitiSize size) 
        {
            foreach (NeedGraffiti ngc in graffitiColliders)
            {
                if (ngc.Sizes.Contains(size))
                {
                    if (ngc.Sizes.Count == 1 && ngc.Sizes[0] == size)
                    {
                        ngc.EnableTrigger();
                        Core.Logger.LogInfo($"Enabled trigger of object {ngc.transform.parent.name}");
                    }
                    else if (ngc.Sizes.Count > 1)
                    {
                        bool hasGraffiti = true;
                        foreach (GraffitiSize s in ngc.Sizes)
                        {
                            if (!Core.Instance.SaveManager.IsAnyGraffitiUnlocked(s))
                            {
                                hasGraffiti = false;
                                break;
                            }
                        }
                        if (hasGraffiti)
                        {
                            ngc.EnableTrigger();
                            Core.Logger.LogInfo($"Enabled trigger of object {ngc.transform.parent.name}");
                        }
                    }
                }
            }
        }

        public virtual void FindStoryObjects(SceneObjectsRegister sceneObjectsRegister) { }
    }
}
