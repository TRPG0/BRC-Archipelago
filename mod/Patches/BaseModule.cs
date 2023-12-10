using HarmonyLib;
using Reptile;
using Archipelago.Stages;
using UnityEngine;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(BaseModule), "ShowMainMenu")]
    public class BaseModule_ShowMainMenu_Patch
    {
        public static void Postfix()
        {
            if (Core.Instance.RandoLocalizer == null)
            {
                LocalizationData localizationData = Reptile.Core.Instance.localizerData;
                Core.Instance.RandoLocalizer = new RandoLocalizer(Reptile.Core.Instance.Localizer.Language, SystemLanguage.English, Core.CheckAvailableLanguages(), localizationData);
            }
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

                foreach (CharacterProgress progress in Traverse.Create(Core.Instance.SaveManager.CurrentSaveSlot).Field<CharacterProgress[]>("totalCharacterProgress").Value)
                {
                    progress.moveStyle = Core.Instance.Data.startingMovestyle;
                }
                Core.Instance.SaveManager.UnlockMaps();
                Core.Instance.SaveManager.CurrentSaveSlot.LockCharacter(Characters.blockGuy);
                Core.Instance.SaveManager.CurrentSaveSlot.LockCharacter(Characters.spaceGirl);
                Core.Instance.SaveManager.CurrentSaveSlot.LockCharacter(Characters.girl1);
                //Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Characters.metalHead).moveStyle = Core.Instance.Data.startingMovestyle;
                //Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Characters.legendMetalHead).moveStyle = Core.Instance.Data.startingMovestyle;
                //Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(Characters.legendFace).moveStyle = Core.Instance.Data.startingMovestyle;
                Core.Instance.SaveManager.CurrentSaveSlot.characterSelectLocked = true;
                Core.Instance.SaveManager.CurrentSaveSlot.cameraAppLocked = true;
                Core.Instance.SaveManager.CurrentSaveSlot.taxiLocked = false;
                return false;
            }
            else return true;
        }
    }

    [HarmonyPatch(typeof(BaseModule), "SaveBeforeStageExit")]
    public class BaseModule_SaveBeforeStageExit_Patch
    {
        public static void Prefix()
        {
            Core.Instance.stageManager = null;
            if (Core.Instance.SaveManager.DataExists())
            {
                Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().reputation = 0;
            }
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
            Core.Instance.UIManager.APMenuChat.gameObject.SetActive(false);
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
                switch (__instance.CurrentStage)
                {
                    case Stage.hideout:
                        Core.Instance.stageManager = new HideoutManager();
                        break;
                    case Stage.downhill:
                        Core.Instance.stageManager = new DownhillManager();
                        break;
                    case Stage.square:
                        Core.Instance.stageManager = new SquareManager();
                        break;
                    case Stage.tower:
                        Core.Instance.stageManager = new TowerManager();
                        break;
                    case Stage.Mall:
                        Core.Instance.stageManager = new MallManager();
                        break;
                    case Stage.pyramid:
                        Core.Instance.stageManager = new PyramidManager();
                        break;
                    case Stage.osaka:
                        Core.Instance.stageManager = new OsakaManager();
                        break;
                    default:
                        break;
                }

                if (Core.Instance.stageManager != null) Core.Instance.stageManager.DoStageSetup();
            }
        }
    }
}
