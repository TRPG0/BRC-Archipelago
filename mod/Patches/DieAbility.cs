using HarmonyLib;
using Reptile;
using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(DieAbility), "OnStartAbility")]
    public class DieAbility_OnStartAbility_Patch
    {
        public static void Prefix()
        {
            if (Core.Instance.Data.deathLink && Core.Instance.Multiworld.DeathLinkService != null && !Core.Instance.Multiworld.DeathLinkKilling) 
                Core.Instance.Multiworld.DeathLinkService.SendDeathLink(new DeathLink(Core.Instance.Data.slot_name));
        }
    }
}
