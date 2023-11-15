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

        public static int maxMessages = 8;

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
            appArchipelago.title = appArchipelago.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            appArchipelago.title.text = "Archipelago";

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
            appArchipelago.text.transform.localPosition = new Vector3(0, 215, 0);
            GameObject.Instantiate(Phone.GetComponentInChildren<AppMusicPlayer>(true).Content.Find("Overlay").Find("OverlayBottom").gameObject, appArchipelago.Content.Find("Overlay")).transform.localPosition = new Vector3(0, -950, 0);
            appArchipelago.bottomLeftText = GameObject.Instantiate(appArchipelago.text.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.bottomLeftText.gameObject.name = "CancelText";
            appArchipelago.bottomLeftText.alignment = TextAlignmentOptions.BottomLeft;
            appArchipelago.bottomLeftGlyph = GameObject.Instantiate(appArchipelago.bottomLeftText.gameObject, appArchipelago.Content).AddComponent<UIButtonGlyphComponent>();
            appArchipelago.bottomLeftGlyph.gameObject.name = "CancelGlyph";
            Traverse leftGlyphAT = Traverse.Create(appArchipelago.bottomLeftGlyph);
            leftGlyphAT.Field<int>("inputActionID").Value = 57;
            leftGlyphAT.Field<TextMeshProUGUI>("localizedGlyphTextComponent").Value = appArchipelago.bottomLeftGlyph.GetComponent<TextMeshProUGUI>();
            appArchipelago.bottomRightText = GameObject.Instantiate(appArchipelago.bottomLeftText.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.bottomRightText.gameObject.name = "AcceptText";
            appArchipelago.bottomRightText.alignment = TextAlignmentOptions.BottomRight;
            appArchipelago.bottomRightGlyph = GameObject.Instantiate(appArchipelago.bottomRightText.gameObject, appArchipelago.Content).AddComponent<UIButtonGlyphComponent>();
            appArchipelago.bottomRightGlyph.gameObject.name = "AcceptGlyph";
            Traverse rightGlyphAT = Traverse.Create(appArchipelago.bottomRightGlyph);
            rightGlyphAT.Field<int>("inputActionID").Value = 29;
            rightGlyphAT.Field<TextMeshProUGUI>("localizedGlyphTextComponent").Value = appArchipelago.bottomRightGlyph.GetComponent<TextMeshProUGUI>();

            appEncounter = GameObject.Instantiate(appArchipelago.gameObject, appArchipelago.transform.parent).AddComponent<AppEncounter>();
            appEncounter.gameObject.name = "AppEncounter";
            Component.Destroy(appEncounter.GetComponent<AppArchipelago>());
            Traverse appET = Traverse.Create(appEncounter);
            appET.Method("Awake").GetValue();
            appET.Field<Phone>("<MyPhone>k__BackingField").Value = Phone;
            appET.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] { };
            appEncounter.Content.Find("Overlay").Find("Icons").Find("AppIcon").GetComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/encounter.png");
            appEncounter.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>().text = "Encounter";
            GameObject.Destroy(appEncounter.Content.Find("Messages").gameObject);
            appEncounter.bottomLeftText = appEncounter.Content.Find("CancelText").GetComponent<TextMeshProUGUI>();
            appEncounter.bottomLeftGlyph = appEncounter.Content.Find("CancelGlyph").GetComponent<UIButtonGlyphComponent>();
            appEncounter.bottomRightText = appEncounter.Content.Find("AcceptText").GetComponent<TextMeshProUGUI>();
            appEncounter.bottomRightGlyph = appEncounter.Content.Find("AcceptGlyph").GetComponent<UIButtonGlyphComponent>();

            appEncounter.centerText = GameObject.Instantiate(appEncounter.bottomLeftText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.centerText.gameObject.name = "Center";
            appEncounter.centerText.alignment = TextAlignmentOptions.Center;
            appEncounter.centerText.transform.localPosition = new Vector3(0, 50, 0);
            appEncounter.topText = GameObject.Instantiate(appEncounter.bottomLeftText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.topText.gameObject.name = "Top";
            appEncounter.topText.alignment = TextAlignmentOptions.Top;
            appEncounter.topText.transform.localPosition = new Vector3(0, -160, 0);
            appEncounter.Content.Find("Overlay").Find("OverlayBottom(Clone)").localPosition = new Vector3(0, -950, 0);
            appEncounter.Init();

            appArchipelago.optionText = GameObject.Instantiate(appArchipelago.text.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.optionText.alignment = TextAlignmentOptions.Center;
            appArchipelago.optionText.lineSpacing = 50;
            appArchipelago.optionText.transform.localPosition = new Vector3(0, 50, 0);
            appArchipelago.optionText.gameObject.SetActive(false);
            appArchipelago.upGlyph = GameObject.Instantiate(appArchipelago.optionText.gameObject, appArchipelago.Content).AddComponent<UIButtonGlyphComponent>();
            appArchipelago.upGlyph.gameObject.name = "UpGlyph";
            appArchipelago.upGlyph.transform.localPosition = new Vector3(0, 450, 0);
            Traverse upGlyphAT = Traverse.Create(appArchipelago.upGlyph);
            upGlyphAT.Field<int>("inputActionID").Value = 21;
            upGlyphAT.Field<TextMeshProUGUI>("localizedGlyphTextComponent").Value = appArchipelago.upGlyph.GetComponent<TextMeshProUGUI>();
            appArchipelago.downGlyph = GameObject.Instantiate(appArchipelago.optionText.gameObject, appArchipelago.Content).AddComponent<UIButtonGlyphComponent>();
            appArchipelago.downGlyph.gameObject.name = "DownGlyph";
            appArchipelago.downGlyph.transform.localPosition = new Vector3(0, -350, 0);
            Traverse downGlyphAT = Traverse.Create(appArchipelago.downGlyph);
            downGlyphAT.Field<int>("inputActionID").Value = 56;
            downGlyphAT.Field<TextMeshProUGUI>("localizedGlyphTextComponent").Value = appArchipelago.downGlyph.GetComponent<TextMeshProUGUI>();

            HomeScreenApp homeScreenAppEncounter = ScriptableObject.CreateInstance<HomeScreenApp>();
            homeScreenAppEncounter.name = "AppEncounter";
            Traverse hsaEncounter = Traverse.Create(homeScreenAppEncounter);
            hsaEncounter.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsaEncounter.Field<string>("m_AppName").Value = "AppEncounter";
            hsaEncounter.Field<string>("m_DisplayName").Value = "ENCOUNTER";
            hsaEncounter.Field<Sprite>("m_AppIcon").Value = UIManager.bundle.LoadAsset<Sprite>("assets/encounter.png");

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
            mainNotification.GetComponentInChildren<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/notification.png");
            mainNotification.GetComponentInChildren<TextMeshProUGUI>().richText = true;

            outsideNotification.GetComponentInChildren<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            Component.Destroy(outsideNotification.GetComponentInChildren<TMProFontLocalizer>());
            outsideNotification.transform.Find("ActionImage").GetComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/notification.png");
            outsideNotification.GetComponentInChildren<TextMeshProUGUI>().richText = true;

            appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value = mainNotification;
            appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value = outsideNotification;
            appAT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Main").Value = mainNotification.GetComponentInChildren<TextMeshProUGUI>();
            appAT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Outside").Value = outsideNotification.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
