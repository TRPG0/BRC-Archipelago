using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Archipelago
{
    public class PhoneManager
    {
        public Phone Phone
        {
            get
            {
                if (Reptile.Core.Instance.BaseModule.IsPlayingInStage)
                {
                    return Traverse.Create(WorldHandler.instance.GetCurrentPlayer()).Field<Phone>("phone").Value;
                }
                else return null;
            }
        }

        public static int maxMessages = 16;

        public AppArchipelago appArchipelago;
        public AppEncounter appEncounter;

        public void DoAppSetup()
        {
            if (!Reptile.Core.Instance.BaseModule.IsPlayingInStage) return;
            if (appArchipelago != null) return;

            appArchipelago = GameObject.Instantiate(Phone.GetComponentInChildren<AppEmail>(true).transform.gameObject, Phone.GetComponentInChildren<AppEmail>(true).transform.parent).AddComponent<AppArchipelago>();
            appArchipelago.gameObject.name = "AppArchipelago";
            Component.DestroyImmediate(appArchipelago.GetComponent<AppEmail>());
            Traverse appAT = Traverse.Create(appArchipelago);
            appAT.Method("Awake").GetValue();
            appAT.Field<Phone>("<MyPhone>k__BackingField").Value = Phone;
            appAT.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] {};
            //appT.Field<Notification>("m_Notification").Value = null;

            GameObject.DestroyImmediate(appArchipelago.Content.Find("EmailScroll").gameObject);
            GameObject.DestroyImmediate(appArchipelago.Content.Find("MessagePanel").gameObject);

            appArchipelago.Content.Find("Overlay").GetComponentInChildren<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            Component.Destroy(appArchipelago.Content.Find("Overlay").GetComponentInChildren<TMProFontLocalizer>());
            Component.Destroy(appArchipelago.Content.Find("Overlay").GetComponentInChildren<TMProLocalizationAddOn>());
            TextMeshProUGUI headerText = appArchipelago.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            headerText.text = "Archipelago";

            Image icon = appArchipelago.Content.Find("Overlay").Find("Icons").Find("AppIcon").GetComponentInChildren<Image>();
            icon.sprite = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");

            GameObject textObject = new GameObject()
            {
                name = "Messages",
                layer = 24,
            };
            textObject.transform.parent = appArchipelago.Content.transform;
            textObject.transform.localPosition = Vector3.zero;
            textObject.transform.SetAsFirstSibling();
            appArchipelago.text = textObject.AddComponent<TextMeshProUGUI>();
            appArchipelago.UpdateText();
            appArchipelago.text.fontSize = 24;
            appArchipelago.text.font = UIManager.font;
            appArchipelago.text.alignment = TextAlignmentOptions.BottomLeft;
            appArchipelago.text.enableWordWrapping = true;
            appArchipelago.GetComponent<RectTransform>().sizeDelta = new Vector2(appArchipelago.transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.95f, appArchipelago.transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.9f);

            appEncounter = GameObject.Instantiate(appArchipelago.gameObject, appArchipelago.transform.parent).AddComponent<AppEncounter>();
            appEncounter.gameObject.name = "AppEncounter";
            Component.Destroy(appEncounter.GetComponent<AppArchipelago>());
            Traverse appET = Traverse.Create(appEncounter);
            appET.Method("Awake").GetValue();
            appET.Field<Phone>("<MyPhone>k__BackingField").Value = Phone;
            appET.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] { };
            appEncounter.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>().text = "Encounter";
            appEncounter.bottomLeftText = appEncounter.Content.Find("Messages").GetComponent<TextMeshProUGUI>();
            appEncounter.bottomLeftText.gameObject.name = "CancelText";
            appEncounter.bottomLeftText.alignment = TextAlignmentOptions.BottomLeft;
            appEncounter.bottomLeftGlyph = GameObject.Instantiate(appEncounter.bottomLeftText.gameObject, appEncounter.Content).AddComponent<UIButtonGlyphComponent>();
            appEncounter.bottomLeftGlyph.gameObject.name = "CancelGlyph";
            Traverse leftGlyphT = Traverse.Create(appEncounter.bottomLeftGlyph);
            leftGlyphT.Field<int>("inputActionID").Value = 57;
            leftGlyphT.Field<TextMeshProUGUI>("localizedGlyphTextComponent").Value = appEncounter.bottomLeftGlyph.GetComponent<TextMeshProUGUI>();
            appEncounter.bottomRightText = GameObject.Instantiate(appEncounter.bottomLeftText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.bottomRightText.gameObject.name = "AcceptText";
            appEncounter.bottomRightText.alignment = TextAlignmentOptions.BottomRight;
            appEncounter.bottomRightGlyph = GameObject.Instantiate(appEncounter.bottomRightText.gameObject, appEncounter.Content).AddComponent<UIButtonGlyphComponent>();
            appEncounter.bottomRightGlyph.gameObject.name = "AcceptGlyph";
            Traverse rightGlyphT = Traverse.Create(appEncounter.bottomRightGlyph);
            rightGlyphT.Field<int>("inputActionID").Value = 29;
            rightGlyphT.Field<TextMeshProUGUI>("localizedGlyphTextComponent").Value = appEncounter.bottomRightGlyph.GetComponent<TextMeshProUGUI>();
            appEncounter.centerText = GameObject.Instantiate(appEncounter.bottomLeftText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.centerText.gameObject.name = "Center";
            appEncounter.centerText.alignment = TextAlignmentOptions.Center;
            appEncounter.topText = GameObject.Instantiate(appEncounter.bottomLeftText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.topText.gameObject.name = "Top";
            appEncounter.topText.alignment = TextAlignmentOptions.Top;
            //appEncounter.bottomTextAddOn = appEncounter.bottomText.gameObject.AddComponent<TMProLocalizationAddOn>();
            //Traverse.Create(appEncounter.bottomTextAddOn).Field<TextMeshProUGUI>("tmpComponent").Value = appEncounter.bottomText;
            appEncounter.Init();

            HomeScreenApp homeScreenAppEncounter = ScriptableObject.CreateInstance<HomeScreenApp>();
            homeScreenAppEncounter.name = "AppEncounter";
            Traverse hsaEncounter = Traverse.Create(homeScreenAppEncounter);
            hsaEncounter.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsaEncounter.Field<string>("m_AppName").Value = "AppEncounter";
            hsaEncounter.Field<string>("m_DisplayName").Value = "ENCOUNTER";
            hsaEncounter.Field<Sprite>("m_AppIcon").Value = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");

            HomeScreenApp homeScreenAppArchipelago = ScriptableObject.CreateInstance<HomeScreenApp>();
            homeScreenAppArchipelago.name = "AppArchipelago";
            Traverse hsaArchipelago = Traverse.Create(homeScreenAppArchipelago);
            hsaArchipelago.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsaArchipelago.Field<string>("m_AppName").Value = "AppArchipelago";
            hsaArchipelago.Field<string>("m_DisplayName").Value = "ARCHIPELAGO";
            hsaArchipelago.Field<Sprite>("m_AppIcon").Value = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");

            Traverse.Create(Phone).Field<Dictionary<string, App>>("<AppInstances>k__BackingField").Value.Add("AppEncounter", appEncounter);
            Traverse.Create(Phone.GetAppInstance<AppHomeScreen>()).Method("AddApp", new object[] { homeScreenAppEncounter }).GetValue();

            Traverse.Create(Phone).Field<Dictionary<string, App>>("<AppInstances>k__BackingField").Value.Add("AppArchipelago", appArchipelago);
            Traverse.Create(Phone.GetAppInstance<AppHomeScreen>()).Method("AddApp", new object[] { homeScreenAppArchipelago }).GetValue();

            GameObject mainGroup = appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value.transform.parent.gameObject;
            GameObject outsideGroup = appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value.transform.parent.gameObject;

            GameObject mainNotification = GameObject.Instantiate(mainGroup.transform.Find("GraffitiNotification").gameObject, mainGroup.transform);
            GameObject outsideNotification = GameObject.Instantiate(outsideGroup.transform.Find("GraffitiNotification").gameObject, outsideGroup.transform);

            mainNotification.GetComponentInChildren<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            Component.Destroy(mainNotification.GetComponentInChildren<TMProFontLocalizer>());
            mainNotification.GetComponentInChildren<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");
            mainNotification.GetComponentInChildren<TextMeshProUGUI>().richText = true;

            outsideNotification.GetComponentInChildren<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            Component.Destroy(outsideNotification.GetComponentInChildren<TMProFontLocalizer>());
            outsideNotification.transform.Find("ActionImage").GetComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");
            outsideNotification.GetComponentInChildren<TextMeshProUGUI>().richText = true;

            appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value = mainNotification;
            appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value = outsideNotification;
            appAT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Main").Value = mainNotification.GetComponentInChildren<TextMeshProUGUI>();
            appAT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Outside").Value = outsideNotification.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
