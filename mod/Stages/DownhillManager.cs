using Reptile;
using System.Collections.Generic;
using UnityEngine;

namespace Archipelago.BRC.Stages
{
    public class DownhillManager : StageManager
    {
        public override string StoryPath => "story_Downhill";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_2;

        public GameObject barricadeChunks;

        public override void FindStoryObjects(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (ProgressObject obj in sceneObjectsRegister.progressObjects)
            {
                if (obj.name == "ProgressObject_Bel")
                {
                    CreateRequirementGraffiti(obj.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M });
                    Core.Logger.LogInfo("Found ProgressObject_Bel");
                }
                else if (obj.name == "BackToPrinceTrigger")
                {
                    CreateRequirementGraffiti(obj.GetComponentInChildren<Collider>(true).gameObject, new List<GraffitiSize>() { GraffitiSize.M });
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
