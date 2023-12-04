using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.Stages
{
    public class DownhillManager : StageManager
    {
        public override string StoryPath => "story_Downhill";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_2;

        public GameObject barricadeChunks;

        public override void FindStoryObjects(StoryManager sm)
        {
            foreach (ProgressObject obj in Traverse.Create(sm).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "ProgressObject_Bel")
                {
                    CreateNeedGraffitiCollider(obj.GetComponentInChildren<Collider>().gameObject, new List<GraffitiSize>() { GraffitiSize.M });
                    Core.Logger.LogInfo("Found ProgressObject_Bel");
                }
                else if (obj.name == "BackToPrinceTrigger")
                {
                    CreateNeedGraffitiCollider(obj.GetComponentInChildren<Collider>().gameObject, new List<GraffitiSize>() { GraffitiSize.M });
                    Core.Logger.LogInfo("Found BackToPrinceTrigger");
                }
                else if (obj.name == "PrinceIntroTrigger")
                {
                    obj.OnExitSequence.AddListener(delegate { DeactivateBarricade(); });
                    Core.Logger.LogInfo("Found PrinceIntroTrigger");
                }
                else if (obj.name == "BarricadeChunks1")
                {
                    barricadeChunks = obj.gameObject;
                    barricadeChunks.SetActive(false);
                    Core.Logger.LogInfo("Found BarricadeChunks1");
                }
            }
        }

        public void DeactivateBarricade()
        {
            if (barricadeChunks == null) return;
            Core.Logger.LogInfo("Deactivated basketball court barricade");
            barricadeChunks.SetActive(false);
        }
    }
}
