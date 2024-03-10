using HarmonyLib;
using Reptile;
using UnityEngine;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(PhotosManager), "SavePhoto")]
    public class PhotosManager_SavePhoto_Patch
    {
        public static bool Prefix(RenderTexture textureToSave, Vector2Int textureSize, out Texture2D outTexture, ref bool __result)
        {
            outTexture = null;
            if (Core.Instance.SaveManager.DataExists() && Core.configDontSavePhotos.Value)
            {
                RenderTexture.active = textureToSave;
                Texture2D texture2D = new Texture2D(textureSize.x, textureSize.y, TextureFormat.RGB24, false);
                texture2D.ReadPixels(new Rect(0f, 0f, textureSize.x, textureSize.y), 0, 0);
                texture2D.Apply();
                RenderTexture.active = null;
                outTexture = texture2D;
                __result = true;
                return false;
            }
            else return true;
        }
    }
}
