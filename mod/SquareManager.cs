using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Archipelago
{
    public class SquareManager : StageManager
    {
        public override bool ChangeScores => false;

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

        public override void DoStageSetup()
        {
            base.DoStageSetup();

            StoryManager sm = WorldHandler.instance.StoryManager;
            DontUnlockCharacterSelect(sm);
        }
    }
}
