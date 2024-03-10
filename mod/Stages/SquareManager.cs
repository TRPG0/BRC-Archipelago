using HarmonyLib;
using Reptile;
using UnityEngine.Events;

namespace Archipelago.BRC.Stages
{
    public class SquareManager : StageManager
    {
        public override bool ChangeScores => false;
        public override bool HasDream => false;
        public override string StoryPath => "story_Square";
        public override Story.Chapter MinimumChapter => Story.Chapter.CHAPTER_1;

        public override void FindStoryObjects(SceneObjectsRegister sceneObjectsRegister)
        {
            foreach (ProgressObject obj in sceneObjectsRegister.progressObjects)
            {
                if (obj.name == "ProgressObject_Phonecall_TeachCypher")
                {
                    Traverse.Create(obj).Field<UnityEvent>("OnTrigger").Value.m_PersistentCalls.Clear();
                    Core.Logger.LogInfo("Removed character select unlock from call with Tryce");
                    break;
                }
            }
        }
    }
}
