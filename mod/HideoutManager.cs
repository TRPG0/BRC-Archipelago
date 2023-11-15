using HarmonyLib;
using Reptile;
using System.Collections.Generic;

namespace Archipelago
{
    public class HideoutManager : StageManager
    {
        public override bool ChangeNPCs => false;
        public override bool ChangeScores => false;

        public void SetSkateboardGarage(bool open)
        {
            foreach (ProgressObject obj in Traverse.Create(WorldHandler.instance.StoryManager).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "GarageDoorSBClosed") obj.gameObject.SetActive(!open);
                if (obj.name == "GarageDoorSBOpen") obj.gameObject.SetActive(open);
            }
        }

        public void SetInlineGarage(bool open)
        {
            foreach (ProgressObject obj in Traverse.Create(WorldHandler.instance.StoryManager).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "GarageDoorInlineClosed") obj.gameObject.SetActive(!open);
                if (obj.name == "GarageDoorInlineOpen") obj.gameObject.SetActive(open);
            }
        }

        public void SetBMXGarage(bool open)
        {
            foreach (ProgressObject obj in Traverse.Create(WorldHandler.instance.StoryManager).Field<List<ProgressObject>>("progressObjects").Value)
            {
                if (obj.name == "GarageDoorBMXClosed") obj.gameObject.SetActive(!open);
                if (obj.name == "GarageDoorBMXOpen") obj.gameObject.SetActive(open);
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

        public override void DoStageSetup()
        {
            base.DoStageSetup();
            SetSkateboardGarage(Core.Instance.Data.skateboardUnlocked);
            SetInlineGarage(Core.Instance.Data.inlineUnlocked);
            SetBMXGarage(Core.Instance.Data.bmxUnlocked);

            StoryManager sm = WorldHandler.instance.StoryManager;
            SetChapter4Character(sm);
        }
    }
}
