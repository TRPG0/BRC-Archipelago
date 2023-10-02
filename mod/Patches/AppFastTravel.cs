using FastTravel;
using HarmonyLib;
using Reptile;
using Reptile.Phone;
using BepInEx;

namespace Archipelago.Patches
{
    [BepInDependency("dance.tari.bombrushcyberfunk.fasttravel")]
    [HarmonyPatch(typeof(AppFastTravel), "OnAppEnable")]
    public class AppFastTravel_OnAppEnable_Patch
    {
        public static bool Prefix(AppFastTravel __instance)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                if (!Core.Instance.SaveManager.CurrentSaveSlot.GetCurrentStageProgress().taxiFound || Traverse.Create(WorldHandler.instance).Field<Encounter>("currentEncounter").Value != null)
                {
                    Core.Logger.LogWarning("Can't use AppFastTravel now.");
                    Traverse.Create(Reptile.Core.Instance.AudioManager).Method("PlaySfxUI", new object[] { SfxCollectionID.MenuSfx, AudioClipID.cancel, 0f }).GetValue();
                    Core.Instance.PhoneManager.Phone.CloseCurrentApp();
                    return false;
                }
                else return true;
            }
            else return true;
        }
    }
}
