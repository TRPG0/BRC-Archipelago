using brc_styleswapmod;
using HarmonyLib;
using Reptile;
using BepInEx;

namespace Archipelago.BRC.Patches
{
    [BepInDependency("com.yuril.brc_styleswapmod")]
    [HarmonyPatch(typeof(brc_styleswapmodPlugin), "SwapStyle")]
    public class brc_styleswapmodPlugin_SwapStyle_Patch
    {
        public static bool Prefix(MoveStyle NewStyle)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                if (NewStyle == MoveStyle.SKATEBOARD && !Core.Instance.Data.skateboardUnlocked)
                {
                    Core.Logger.LogWarning("Can't quick swap to skateboard because it is not unlocked. (Archipelago.Patches.brc_styleswapmodPlugin_SwapStyle_Patch)");
                    return false;
                }
                else if (NewStyle == MoveStyle.INLINE && !Core.Instance.Data.inlineUnlocked)
                {
                    Core.Logger.LogWarning("Can't quick swap to inline skates because it is not unlocked. (Archipelago.Patches.brc_styleswapmodPlugin_SwapStyle_Patch)");
                    return false;
                }
                else if (NewStyle == MoveStyle.BMX && !Core.Instance.Data.bmxUnlocked)
                {
                    Core.Logger.LogWarning("Can't quick swap to BMX because it is not unlocked. (Archipelago.Patches.brc_styleswapmodPlugin_SwapStyle_Patch)");
                    return false;
                }
                else return true;
            }
            else return true;
        }
    }
}
