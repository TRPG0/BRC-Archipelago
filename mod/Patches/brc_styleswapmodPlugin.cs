using brc_styleswapmod;
using HarmonyLib;
using Reptile;
using BepInEx;

namespace Archipelago.Patches
{
    [BepInDependency("com.yuril.brc_styleswapmod")]
    [HarmonyPatch(typeof(brc_styleswapmodPlugin), "SwapStyle")]
    public class brc_styleswapmodPlugin_SwapStyle_Patch
    {
        public static bool Prefix(MoveStyle NewStyle)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                Core.Logger.LogInfo($"Attempted swap to {NewStyle}");
                if (NewStyle == MoveStyle.SKATEBOARD && !Core.Instance.Data.skateboardUnlocked) return false;
                else if (NewStyle == MoveStyle.INLINE && !Core.Instance.Data.inlineUnlocked) return false;
                else if (NewStyle == MoveStyle.BMX && !Core.Instance.Data.bmxUnlocked) return false;
                else return true;
            }
            else return true;
        }
    }
}
