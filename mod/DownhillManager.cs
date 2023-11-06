using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago
{
    public class DownhillManager : StageManager
    {
        public GameObject barricadeChunks;
        public GameObject progressObjectBel;
        public GameObject npcBel;
        public GameObject princeCrewBattleTrigger;

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

        public override void DoStageSetup()
        {
            base.DoStageSetup();

            StoryManager sm = Traverse.Create(WorldHandler.instance).Field<StoryManager>("storyManager").Value;
            FindChapter1Objects(sm);
            AddCallToPrinceCutscene(sm);
            FindPrinceCrewBattleTrigger(sm);
            SetCrewBattleScore(sm, 1500000);
            SkipDream(sm);
        }

        public override void NoGraffitiM()
        {
            DeactivateChapter1Objects();
            DeactivatePrinceCrewBattleTrigger();
        }

        public override void YesGraffitiM()
        {
            ActivateChapter1Objects();
            ActivatePrinceCrewBattleTrigger();
        }
    }
}
