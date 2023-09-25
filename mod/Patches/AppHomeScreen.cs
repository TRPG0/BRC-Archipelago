using DG.Tweening;
using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System.Linq;
using UnityEngine;

namespace Archipelago.Patches
{
    [HarmonyPatch(typeof(AppHomeScreen), "OpenApp")]
    public class AppHomeScreen_OpenApp_Patch
    {
        public static bool Prefix(AppHomeScreen __instance, HomescreenButton appToOpen)
        {
            if (appToOpen.AssignedApp.AppName == "AppArchipelago")
            {
                Traverse thisHome = Traverse.Create(__instance);
                Sequence sequence = DOTween.Sequence();
                Sequence sequence2 = DOTween.Sequence();
                Sequence sequence3 = DOTween.Sequence();
                Sequence sequence4 = DOTween.Sequence();
                if (appToOpen.AssignedApp.AppType == HomeScreenApp.HomeScreenAppType.PHOTO_ALBUM)
                {
                    thisHome.Property<Phone>("MyPhone").Value.GetAppInstance<AppPhotos>().OnAppEnable();
                }
                for (int i = thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.GetScrollRange() - 1; i >= 0; i--)
                {
                    RectTransform target = thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.GetButtonByRelativeIndex(i).RectTransform();
                    if (i != thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.GetSelectorPos())
                    {
                        sequence2.Append(target.DOAnchorPosX(-thisHome.Property<Phone>("MyPhone").Value.ScreenSize.x, 0.06f, false).SetEase(Ease.Linear));
                    }
                }
                sequence4.AppendCallback(delegate
                {
                    thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.Selector.RectTransform().DOKill(false);
                });
                sequence4.Append(thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.Selector.RectTransform().DOAnchorPosX(0f, 0.3f, false).SetEase(Ease.OutBounce));
                int num = 2;
                float interval = 0.1f;
                for (int j = 0; j < num; j++)
                {
                    sequence3.Append(thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.Selector.Background.DOFade(0f, 0f));
                    sequence3.AppendInterval(interval);
                    sequence3.Append(thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.Selector.Background.DOFade(1f, 0f));
                    sequence3.AppendInterval(interval);
                }
                sequence3.Append(thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.Selector.RectTransform().DOScale(0f, 0f));
                sequence3.Append(thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.SelectedButtton.RectTransform().DOAnchorPosX(thisHome.Property<Phone>("MyPhone").Value.ScreenSize.x, 0f, false));
                sequence3.AppendCallback(delegate
                {
                    thisHome.Field<HomescreenScrollView>("m_ScrollView").Value.OtherElementsParent.gameObject.SetActive(false);
                });
                sequence.AppendCallback(delegate
                {
                    thisHome.Field<bool>("<HandleInput>k__BackingField").Value = false;
                });
                sequence.Append(sequence2);
                sequence.Join(sequence4);
                sequence.Join(sequence3);
                sequence.Append(thisHome.Field<RectTransform>("m_TopView").Value.DOSizeDelta(new Vector2(thisHome.Field<RectTransform>("m_TopView").Value.sizeDelta.x, 215f), 0.06f, false)).SetEase(Ease.Linear);
                sequence.AppendCallback(delegate
                {
                    thisHome.Field<bool>("<HandleInput>k__BackingField").Value = true;
                });
                sequence.OnComplete(delegate
                {
                    thisHome.Property<Phone>("MyPhone").Value.OpenApp(typeof(AppArchipelago));
                });
                Traverse.Create(thisHome.Field<AudioManager>("m_AudioManager").Value).Method("PlaySfxGameplay", new object[] { SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Confirm, 0f }).GetValue();
                return false;
            }
            else return true;
        }
    }
}
