using HarmonyLib;

namespace Archipelago.Patches
{
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
