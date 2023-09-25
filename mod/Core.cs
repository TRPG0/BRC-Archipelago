using Archipelago.Patches;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace Archipelago
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency("com.yuril.brc_styleswapmod", BepInDependency.DependencyFlags.SoftDependency)]
    public class Core : BaseUnityPlugin
    {
        public const string PluginGUID = "trpg.brc.archipelago";
        public const string PluginName = "Archipelago";
        public const string PluginVersion = "1.0.0";

        public static Core Instance;
        public Data Data = new Data();
        public UIManager UIManager = new UIManager();
        public PhoneManager PhoneManager = new PhoneManager();
        public Multiworld Multiworld = new Multiworld();
        public LocationManager LocationManager = new LocationManager();
        public SaveManager SaveManager = new SaveManager();
        public WorldManager WorldManager = new WorldManager();

        public static new ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("Archipelago");

        public static ConfigEntry<Color> configColorPlayerSelf;
        public static ConfigEntry<Color> configColorPlayerOther;
        public static ConfigEntry<Color> configColorItemAdvancement;
        public static ConfigEntry<Color> configColorItemNeverExclude;
        public static ConfigEntry<Color> configColorItemFiller;
        public static ConfigEntry<Color> configColorItemTrap;
        public static ConfigEntry<Color> configColorLocation;

        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Instance = this;

            Harmony Harmony = new Harmony("Archipelago");
            Harmony.PatchAll(typeof(AppGraffiti_OnHoldDown_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnHoldUp_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnPressDown_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnPressRight_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnPressUp_Patch));
            Harmony.PatchAll(typeof(AppGraffiti_OnReleaseRight_Patch));
            Harmony.PatchAll(typeof(AppHomeScreen_OpenApp_Patch));
            Harmony.PatchAll(typeof(BaseModule_HandleStageFullyLoaded_Patch));
            Harmony.PatchAll(typeof(BaseModule_QuitCurrentGameSession_Patch));
            Harmony.PatchAll(typeof(BaseModule_ShowMainMenu_Patch));
            Harmony.PatchAll(typeof(BaseModule_StartNewGame_Patch));
            Harmony.PatchAll(typeof(CharacterSelect_PopulateListOfSelectableCharacters_Patch));
            Harmony.PatchAll(typeof(Collectable_PickupCollectable_Patch));
            Harmony.PatchAll(typeof(DynamicPickup_PickupPickup_Patch));
            Harmony.PatchAll(typeof(GraffitiSpot_GiveRep_Patch));
            Harmony.PatchAll(typeof(GraffitiSpot_SpawnRep_Patch));
            Harmony.PatchAll(typeof(SaveManager_SaveCurrentSaveSlot_Patch));
            Harmony.PatchAll(typeof(SaveSlotData_UnlockCharacter_Patch));
            Harmony.PatchAll(typeof(SaveSlotMenu_OnPressedPopupDeleteButton_Patch));

            foreach (var plugin in Chainloader.PluginInfos)
            {
                if (plugin.Value.Metadata.GUID == "com.yuril.brc_styleswapmod")
                {
                    Logger.LogInfo($"QuickStyleSwap is installed. Patching \"SwapStyle\"");
                    Harmony.PatchAll(typeof(brc_styleswapmodPlugin_SwapStyle_Patch));
                    break;
                }
            }

            Reptile.Core.OnCoreInitialized += SaveManager.GetSavePath;

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
        }
    }
}
