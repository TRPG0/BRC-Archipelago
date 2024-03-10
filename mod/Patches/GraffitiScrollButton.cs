using HarmonyLib;
using Reptile;
using Reptile.Phone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Archipelago.BRC.Structures;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(GraffitiScrollButton), "SetContent")]
    public class GraffitiScrollButton_SetContent_Patch
    {
        public static void Postfix(AUnlockable content, GraffitiScrollButton __instance)
        {
            if (Core.Instance.SaveManager.DataExists() && Core.Instance.Data.limitedGraffiti)
            {
                GraffitiAppEntry graffiti = __instance.AssignedContent as GraffitiAppEntry;
                Traverse traverse = Traverse.Create(__instance);
                int uses = 0;
                switch (graffiti.Size)
                {
                    case GraffitiSize.S:
                        traverse.Field<RawImage>("m_GraffitiImage").Value.rectTransform.sizeDelta = new Vector2(256f, 256f);
                        if (Core.Instance.Data.sGraffiti == SGraffiti.Separate) uses = Requirements.grafSLimit - Core.Instance.Data.grafUses[graffiti.Uid];
                        else uses = Core.Instance.Data.sMax - Core.Instance.Data.grafUses["S"];
                        break;
                    case GraffitiSize.M:
                        uses = Requirements.grafMLimit - Core.Instance.Data.grafUses[graffiti.Uid];
                        break;
                    case GraffitiSize.L:
                        uses = Requirements.grafLLimit - Core.Instance.Data.grafUses[graffiti.Uid];
                        break;
                    case GraffitiSize.XL:
                        uses = Requirements.grafXLLimit - Core.Instance.Data.grafUses[graffiti.Uid];
                        break;
                    default:
                        break;
                }
                traverse.Field<TextMeshProUGUI>("m_TitleLabel").Value.text = $"{graffiti.Title} ({uses})";
            }
        }
    }
}
