using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(BaseModule), "ShowMainMenu")]
    public class BaseModule_ShowMainMenu_Patch
    {
        public static void Postfix()
        {
            Core.Instance.UIManager.CreateAPButtons();
            Core.Instance.UIManager.CheckSlots();
        }
    }

    [HarmonyPatch(typeof(BaseModule), "StartNewGame")]
    public class BaseModule_StartNewGame_Patch
    {
        public static bool Prefix(BaseModule __instance)
        {
            Core.Instance.UIManager.HideMenu();
            if (Core.Instance.Multiworld.Authenticated)
            {
                if (Core.Instance.Data.skipIntro)
                {
                    __instance.LoadStage(Stage.hideout);
                    Core.Instance.SaveManager.CurrentSaveSlot.CurrentStoryObjective = Story.ObjectiveID.JoinTheCrew;
                }
                else __instance.LoadStage(Stage.Prelude);
                Core.Instance.WorldManager.UnlockMaps(Core.Instance.SaveManager.CurrentSaveSlot);
                Core.Instance.SaveManager.CurrentSaveSlot.LockCharacter(Characters.blockGuy);
                Core.Instance.SaveManager.CurrentSaveSlot.LockCharacter(Characters.spaceGirl);
                Core.Instance.SaveManager.CurrentSaveSlot.LockCharacter(Characters.girl1);
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Characters.metalHead).moveStyle = Core.Instance.Data.startingMovestyle;
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Characters.legendMetalHead).moveStyle = Core.Instance.Data.startingMovestyle;
                Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Characters.legendFace).moveStyle = Core.Instance.Data.startingMovestyle;
                Core.Instance.SaveManager.CurrentSaveSlot.characterSelectLocked = true;
                Core.Instance.SaveManager.CurrentSaveSlot.cameraAppLocked = true;
                Core.Instance.SaveManager.CurrentSaveSlot.taxiLocked = false;
                return false;
            }
            else return true;
        }
    }

    [HarmonyPatch(typeof(BaseModule), "QuitCurrentGameSession")]
    public class BaseModule_QuitCurrentGameSession_Patch
    {
        public static void Postfix()
        {
            if (Core.Instance.Multiworld.Authenticated) Core.Instance.Multiworld.Disconnect();
            Core.Instance.LocationManager.locations.Clear();
            Core.Instance.UIManager.connectingSlot = -1;
            Core.Instance.UIManager.SetResult(string.Empty);
            Core.Instance.UIManager.HideMenu();
            Core.Instance.SaveManager.currentSlot = -1;
        }
    }

    [HarmonyPatch(typeof(BaseModule), "HandleStageFullyLoaded")]
    public class BaseModule_HandleStageFullyLoaded_Patch
    {
        public static void Postfix(BaseModule __instance)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                if (__instance.CurrentStage == Stage.hideout) Core.Instance.WorldManager.SetGarages();
                if (__instance.CurrentStage != Stage.Prelude) Core.Instance.WorldManager.LockDefaultGraffiti(Core.Instance.Data.to_lock);
                Core.Instance.PhoneManager.DoAppSetup();
                Core.Instance.WorldManager.DoStageSetup();
                //Core.Instance.LocationManager.GetQueuedItems();
                //Core.Instance.LocationManager.PushQueuedNotifications();
            }
        }
    }
}
