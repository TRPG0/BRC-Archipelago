using HarmonyLib;
using Reptile.Phone;
using System.Collections;

namespace Archipelago.BRC.Patches
{
    [HarmonyPatch(typeof(AppCamera), "WaitForPhotoSavingComplete")]
    public class AppCamera_WaitForPhotoSavingComplete
    {
        class SkipSaving : IEnumerable
        {
            public AppCamera appCamera;
            IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

            public IEnumerator GetEnumerator()
            {
                Traverse traverse = Traverse.Create(appCamera);
                traverse.Method("TriggerPostPhotoSaveSuccesEvent").GetValue();
                traverse.Field<bool>("m_TakingPhoto").Value = false;
                yield break;
            }
        }
        

        public static bool Prefix(AppCamera __instance, ref IEnumerator __result)
        {
            if (Core.Instance.SaveManager.DataExists() && Core.configDontSavePhotos.Value)
            {
                var ss = new SkipSaving()
                {
                    appCamera = __instance
                };
                __result = ss.GetEnumerator();
                return false;
            }
            else return true;
        }
    }
}
