﻿using HarmonyLib;
using Reptile;
using System;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(GraffitiSpot), "GiveRep")]
    public class GraffitiSpot_GiveRep_Patch
    {
        public static bool Prefix(GraffitiSpot __instance)
        {
            if (Core.Instance.SaveManager.DataExists())
            {
                if (__instance.bottomCrew != __instance.topCrew 
                    && (__instance.topCrew == Crew.PLAYERS || __instance.topCrew == Crew.ROGUE) 
                    && Core.Instance.stageManager != null)
                {
                    Core.Instance.LocationManager.CountAndCheckSpray();
                    return false;
                }
                else return false;
            }
            else return true;
        }
    }

    [HarmonyPatch(typeof(GraffitiSpot), "SpawnRep")]
    public class GraffitiSpot_SpawnRep_Patch
    {
        public static bool Prefix()
        {
            if (Core.Instance.SaveManager.DataExists()) return false;
            else return true;
        }
    }
}
