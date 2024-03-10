using HarmonyLib;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(Reptile.UIManager), "InstantiateMainMenuSceneUI")]
    public class UIManager_InstantiateMainMenuSceneUI_Patch
    {
        public static void Postfix()
        {
            Core.Instance.UIManager.CreateAPButtons();
            Core.Instance.UIManager.CheckSlots();
        }
    }

    [HarmonyPatch(typeof(Reptile.UIManager), "HideDieMenu")]
    public class UIManager_HideDieMenu_Patch
    {
        public static void Prefix()
        {
            Core.Instance.Multiworld.DeathLinkKilling = false;
        }
    }

    [HarmonyPatch(typeof(Reptile.UIManager), "ShowDieMenu")]
    public class UIManager_ShowDieMenu_Patch
    {
        public static void Prefix()
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                if (Core.Instance.Multiworld.DeathLinkKilling) Core.Instance.stageManager.dieMenuReasonLabel.gameObject.SetActive(true);
            }
        }
    }
}
