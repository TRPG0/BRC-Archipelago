using Archipelago.BRC.Patches;
using StageManager = Archipelago.BRC.Stages.StageManager;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Reptile;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using ModLocalizer;

namespace Archipelago.BRC
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.yuril.brc_styleswapmod", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("trpg.brc.modlocalizer")]
    public class Core : BaseUnityPlugin
    {
        public const string PluginGUID = "trpg.brc.archipelago";
        public const string PluginName = "Archipelago";
        public const string PluginVersion = "1.0.0";
        internal static GameVersion TargetGameVersion;

        public static Core Instance;
        public Data Data = new Data();
        public UIManager UIManager = new UIManager();
        public PhoneManager PhoneManager = new PhoneManager();
        public Multiworld Multiworld = new Multiworld();
        public LocationManager LocationManager = new LocationManager();
        public SaveManager SaveManager = new SaveManager();
        public StageManager stageManager = null;

        public PluginLocalizer Localizer { get; private set; }
        public static GameFontType MainFont { get; private set; }
        public static GameFontType MainFontEnglish { get; private set; }
        public static GameFontType PhoneFont { get; private set; }

        public static new ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Archipelago");

        public static ConfigEntry<string> configDefaultName;
        public static ConfigEntry<string> configDefaultAddress;
        public static ConfigEntry<string> configDefaultPassword;
        public static ConfigEntry<Color> configColorPlayerSelf;
        public static ConfigEntry<Color> configColorPlayerOther;
        public static ConfigEntry<Color> configColorItemAdvancement;
        public static ConfigEntry<Color> configColorItemNeverExclude;
        public static ConfigEntry<Color> configColorItemFiller;
        public static ConfigEntry<Color> configColorItemTrap;
        public static ConfigEntry<Color> configColorLocation;
        public static ConfigEntry<bool> configDontSavePhotos;

        public static bool isQuickStyleSwapLoaded = false;

        public static bool FailScoreEncounters { get; private set; } = false;

        internal static int forbiddenModsLoaded = 0;
        internal static string forbiddenGUIDs = string.Empty;
        private readonly List<string> forbiddenMods = new List<string>()
        {
            "QuickGraffiti",
            "com.Dragsun.FastCypher",
            "dance.tari.bombrushcyberfunk.customgraffiti",
            "TombRush"
        };

        private void CheckForbiddenMods()
        {
            forbiddenModsLoaded = 0;
            forbiddenGUIDs = string.Empty;
            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (forbiddenMods.Contains(plugin.Value.Metadata.GUID))
                {
                    forbiddenModsLoaded++;
                    Logger.LogWarning($"A forbidden mod is loaded. (\"{plugin.Value.Metadata.GUID}\")");
                    if (forbiddenGUIDs == string.Empty) forbiddenGUIDs = $"\"{plugin.Value.Metadata.GUID}\"";
                    else forbiddenGUIDs = forbiddenGUIDs += $", \"{plugin.Value.Metadata.GUID}\"";
                }
            }
        }

        private void Awake()
        {
            if (Instance != null) Destroy(this);
            Instance = this;
            TargetGameVersion = ScriptableObject.CreateInstance<GameVersion>();
            TargetGameVersion.major = 1;
            TargetGameVersion.minor = 0;
            TargetGameVersion.repositoryRevision = 20381;

            string assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Localizer = new PluginLocalizer(PluginName, Path.Combine(assemblyPath, "Languages"));
            Localizer.OnLanguageChanged += UIManager.UpdateLanguage;
            Localizer.OnInitializationFinished += () => {
                MainFont = Localizer.LoadFontTypeAndRemoveUnusedFonts(GameFontTypes.DefaultText);
                MainFontEnglish = Localizer.LoadFontTypeEnglishOnly(GameFontTypes.DefaultText);
                PhoneFont = Localizer.LoadFontTypeAndRemoveUnusedFonts(GameFontTypes.PhoneMainText);
            };

            Harmony Harmony = new Harmony("Archipelago");
            Harmony.PatchAll(typeof(AppCamera_WaitForPhotoSavingComplete));
            Harmony.PatchAll(typeof(AppGraffiti_OnHoldDown_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnHoldUp_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnPressDown_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnPressRight_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnPressUp_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnReleaseRight_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OpenInfo_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_RefreshList_Patch));
            Harmony.PatchAll(typeof(BaseModule_HandleStageFullyLoaded_Patch));
            Harmony.PatchAll(typeof(BaseModule_SaveBeforeStageExit_Patch));
            Harmony.PatchAll(typeof(BaseModule_QuitCurrentGameSession_Patch));
            Harmony.PatchAll(typeof(BaseModule_StartNewGame_Patch));
            Harmony.PatchAll(typeof(CharacterSelect_PopulateListOfSelectableCharacters_Patch));
            Harmony.PatchAll(typeof(Collectable_PickupCollectable_Patch));
            Harmony.PatchAll(typeof(DieAbility_OnStartAbility_Patch));
            Harmony.PatchAll(typeof(DynamicPickup_PickupPickup_Patch));
            Harmony.PatchAll(typeof(Encounter_SetEncounterState_Patch));
            Harmony.PatchAll(typeof(GameplayEvent_UnlockCameraAppForPlayer_Patch));
            Harmony.PatchAll(typeof(GameplayEvent_UnlockCharacterImpl_Patch));
            Harmony.PatchAll(typeof(GraffitiGame_OnDestroy_Patch));
            Harmony.PatchAll(typeof(GraffitiGame_SetState_Patch));
            Harmony.PatchAll(typeof(GraffitiGame_SetStateVisual_Patch));
            Harmony.PatchAll(typeof(GraffitiInfoPanel_Show_Patch));
            Harmony.PatchAll(typeof(GraffitiScrollButton_SetContent_Patch));
            Harmony.PatchAll(typeof(GraffitiSpot_GiveRep_Patch));
            Harmony.PatchAll(typeof(GraffitiSpot_SpawnRep_Patch));
            Harmony.PatchAll(typeof(HomescreenButton_SetContent_Patch));
            Harmony.PatchAll(typeof(LocalizationLookupTable_GetLocalizationValueFromSubgroup_Patch));
            Harmony.PatchAll(typeof(PhoneScrollUnlockableButton_SetContent_Patch));
            Harmony.PatchAll(typeof(PhotoObjectiveProgressable_MadePhotograph_Patch));
            Harmony.PatchAll(typeof(PhotosManager_SavePhoto_Patch));
            Harmony.PatchAll(typeof(Player_ChangeHP_Patch));
            Harmony.PatchAll(typeof(Player_CheckNPCTriggerForConversation_Patch));
            Harmony.PatchAll(typeof(Player_OnTriggerEnter_Patch));
            Harmony.PatchAll(typeof(Player_OnTriggerExit_Patch));
            Harmony.PatchAll(typeof(Player_StartGraffitiMode_Patch));
            Harmony.PatchAll(typeof(ScoreEncounter_StartMainEvent_Patch));
            Harmony.PatchAll(typeof(SaveManager_SaveCurrentSaveSlot_Patch));
            Harmony.PatchAll(typeof(SaveSlotData_UnlockCharacter_Patch));
            Harmony.PatchAll(typeof(SaveSlotMenu_OnPressedPopupDeleteButton_Patch));
            Harmony.PatchAll(typeof(SaveSlotMenu_ShowDeleteSaveSlotPopup_Patch));
            Harmony.PatchAll(typeof(TextMeshProGameTextLocalizer_GetRawDialogueTextValue_Patch));
            Harmony.PatchAll(typeof(Type_GetType_Patch));
            Harmony.PatchAll(typeof(UIManager_InstantiateMainMenuSceneUI_Patch));
            Harmony.PatchAll(typeof(UIManager_HideDieMenu_Patch));
            Harmony.PatchAll(typeof(UIManager_ShowDieMenu_Patch));
            Harmony.PatchAll(typeof(VendingMachine_RewardIsValid_Patch));
            Harmony.PatchAll(typeof(VersionUIHandler_SetVersionText_Patch));

            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Metadata.GUID == "com.yuril.brc_styleswapmod")
                {
                    isQuickStyleSwapLoaded = true;
                    Logger.LogInfo("QuickStyleSwap is loaded. Applying SwapStyle patch.");
                    Harmony.PatchAll(typeof(brc_styleswapmodPlugin_SwapStyle_Patch));

                }
            }

            Reptile.Core.OnCoreInitialized += SaveManager.GetSavePath;
            Reptile.Core.OnCoreInitialized += CheckForbiddenMods;

            configDefaultName = Config.Bind("Defaults",
                "defaultName",
                "Red",
                "The default name used when opening the connection menu for a slate with no save data.");

            configDefaultAddress = Config.Bind("Defaults",
                "defaultAddress",
                "archipelago.gg",
                "The default address used when opening the connection menu for a slate with no save data.");

            configDefaultPassword = Config.Bind("Defaults",
                "defaultPassword",
                "",
                "The default password used when opening the connection menu for a slate with no save data.");

            configColorPlayerSelf = Config.Bind("Colors", 
                "colorPlayerSelf", 
                new Color(0.93f, 0, 0.93f), 
                "The color used to represent you.");

            configColorPlayerOther = Config.Bind("Colors",
                "colorPlayerOther",
                new Color(0.98f, 0.98f, 0.92f),
                "The color used to represent other players.");

            configColorItemAdvancement = Config.Bind("Colors",
                "colorItemAdvancement",
                new Color(0.69f, 0.6f, 0.94f),
                "The color used to represent items that are important for progression.");

            configColorItemNeverExclude = Config.Bind("Colors",
                "colorItemNeverExclude",
                new Color(0.43f, 0.55f, 0.91f),
                "The color used to represent items that are useful.");

            configColorItemFiller = Config.Bind("Colors",
                "colorItemFiller",
                new Color(0, 0.93f, 0.93f),
                "The color used to represent items that are not very important.");

            configColorItemTrap = Config.Bind("Colors",
                "colorItemTrap",
                new Color(0.98f, 0.5f, 0.45f),
                "The color used to represent items that are traps.");

            configColorLocation = Config.Bind("Colors",
                "colorLocation",
                new Color(0, 1, 0.5f),
                "The color used to represent locations.");

            configDontSavePhotos = Config.Bind("Camera",
                "dontSavePhotos",
                false,
                "Don't save photos taken with the Camera app. Only takes effect when playing randomizer.");
        }
    }
}
