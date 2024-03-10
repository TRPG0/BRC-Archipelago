using HarmonyLib;
using Reptile;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(SaveSlotMenu), "OnPressedPopupDeleteButton")]
    public class SaveSlotMenu_OnPressedPopupDeleteButton_Patch
    {
        public static void Prefix(SaveSlotMenu __instance)
        {
            int slot = Traverse.Create(__instance).Field<int>("deletingSlotId").Value;
            Core.Instance.SaveManager.DeleteData(slot);
            Core.Instance.UIManager.slotButtons[slot].ChangeState(Components.APSlotButton.SlotState.NoData);
        }
    }

    [HarmonyPatch(typeof(SaveSlotMenu), "ShowDeleteSaveSlotPopup")]
    public class SaveSlotMenu_ShowDeleteSaveSlotPopup_Patch
    {
        public static void Prefix()
        {
            Core.Instance.UIManager.HideMenu();
            Core.Instance.UIManager.SetResult(string.Empty);
        }
    }
}
