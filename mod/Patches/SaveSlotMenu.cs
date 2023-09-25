using HarmonyLib;
using Reptile;

namespace Archipelago.Patches
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
}
