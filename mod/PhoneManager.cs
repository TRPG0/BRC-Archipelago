using HarmonyLib;
using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Archipelago.Apps;
using ModLocalizer;

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

        public static Color PhoneBlue => new Color(0.224f, 0.302f, 0.624f);
        public static Color PhoneYellow => new Color(0.882f, 0.953f, 0.345f);
        public static Color PhoneOrange => new Color(0.978f, 0.424f, 0.216f);

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

            Component.Destroy(appArchipelago.Content.Find("Overlay").GetComponentInChildren<TMProLocalizationAddOn>());
            appArchipelago.title = appArchipelago.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            appArchipelago.title.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_HEADER_DEFAULT");

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
            appArchipelago.text.fontSize = 68;
            appArchipelago.text.font = UIManager.font;
            appArchipelago.text.alignment = TextAlignmentOptions.BottomLeft;
            appArchipelago.text.enableWordWrapping = true;
            appArchipelago.text.transform.localPosition = new Vector3(0, 250, 0);
            appArchipelago.text.transform.localScale = Vector3.one;
            appArchipelago.text.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 1560);
            TMProFontLocalizer fontLocalizer = appArchipelago.text.gameObject.AddComponent<TMProFontLocalizer>();
            Traverse fontLocalizerT = Traverse.Create(fontLocalizer);
            fontLocalizerT.Field<TextMeshProUGUI>("textMesh").Value = appArchipelago.text;
            fontLocalizerT.Field<GameFontType>("gameFontType").Value = Core.MainFont;

            appArchipelago.chatBackground = GameObject.Instantiate(Phone.GetAppInstance<AppHomeScreen>().Content.Find("BottomView").Find("ButtonContainer").Find("Selector").Find("Background").gameObject, appArchipelago.Content);
            appArchipelago.chatBackground.transform.localPosition = new Vector3(100, -650, 0);
            appArchipelago.chatBackground.name = "Chat Bottom";

            // do encounter setup now instead of deleting all objects later
            appEncounter = GameObject.Instantiate(appArchipelago.gameObject, appArchipelago.transform.parent).AddComponent<AppEncounter>();
            appEncounter.gameObject.name = "AppEncounter";
            Component.Destroy(appEncounter.GetComponent<AppArchipelago>());
            Traverse appET = Traverse.Create(appEncounter);
            appET.Method("Awake").GetValue();
            appET.Field<Phone>("<MyPhone>k__BackingField").Value = Phone;
            appET.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] { };
            appEncounter.Content.Find("Overlay").Find("Icons").Find("AppIcon").GetComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/encounter.png");
            appEncounter.headerText = appEncounter.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            appEncounter.headerText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_HEADER");
            appEncounter.Init();
            GameObject.Destroy(appEncounter.Content.Find("Messages").gameObject);

            GameObject encBackground = appEncounter.Content.Find("Chat Bottom").gameObject;
            encBackground.name = "Text Background";
            encBackground.transform.Rotate(0, 0, 180);
            encBackground.transform.localPosition = new Vector3(-100, 500, 0);

            GameObject encImage = new GameObject()
            {
                name = "Background Image",
                layer = 24
            };
            encImage.transform.SetParent(appEncounter.Content);
            encImage.transform.localPosition = new Vector3(250, -250, 0);
            encImage.transform.localScale = new Vector3(12, 12, 12);
            encImage.AddComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/encounter_large.png");
            encImage.GetComponent<Image>().color = PhoneBlue;

            GameObject encTop = GameObject.Instantiate(appArchipelago.chatBackground, appEncounter.Content);
            Component.DestroyImmediate(encTop.GetComponent<Image>());
            encTop.transform.localPosition = new Vector3(75, 500, 0);
            encTop.name = "Current Encounter";
            appEncounter.currentText = encTop.AddComponent<TextMeshProUGUI>();
            Traverse tmpfl = Traverse.Create(appEncounter.currentText.gameObject.AddComponent<TMProFontLocalizer>());
            tmpfl.Field<GameFontType>("gameFontType").Value = Core.PhoneFont;
            tmpfl.Field<TextMeshProUGUI>("textMesh").Value = appEncounter.currentText;
            appEncounter.currentText.alignment = TextAlignmentOptions.Left;
            appEncounter.currentText.fontSize = 80;
            appEncounter.currentText.color = PhoneYellow;
            appEncounter.currentText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_TOP");

            appEncounter.messageText = GameObject.Instantiate(appEncounter.currentText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.messageText.GetComponent<RectTransform>().sizeDelta = new Vector2(-200, 1);
            appEncounter.messageText.alignment = TextAlignmentOptions.TopLeft;
            appEncounter.messageText.color = Color.white;
            appEncounter.messageText.enableWordWrapping = true;
            appEncounter.messageText.transform.localPosition = new Vector3(0, -550, 0);

            appEncounter.confirmText = GameObject.Instantiate(appEncounter.currentText.gameObject, appEncounter.Content).GetComponent<TextMeshProUGUI>();
            appEncounter.confirmText.alignment = TextAlignmentOptions.Right;
            appEncounter.confirmText.color = PhoneOrange;
            appEncounter.confirmText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_CONFIRM");

            appEncounter.confirmArrow = GameObject.Instantiate(appEncounter.Content.Find("Overlay").Find("Icons").Find("Arrow").gameObject, appEncounter.Content);
            appEncounter.confirmArrow.transform.Rotate(0, 0, 180);
            appEncounter.confirmArrow.name = "Arrow Right";

            // back to main app
            appArchipelago.headerArrow = appArchipelago.Content.Find("Overlay").Find("Icons").Find("Arrow").gameObject;

            appArchipelago.arrowRight = GameObject.Instantiate(appArchipelago.headerArrow, appArchipelago.Content);
            appArchipelago.arrowRight.transform.Rotate(0, 0, 180);
            appArchipelago.arrowRight.transform.localPosition = new Vector3(450, -650, 0);
            appArchipelago.arrowRight.name = "Arrow Right";

            GameObject swapText = GameObject.Instantiate(appArchipelago.chatBackground, appArchipelago.Content);
            Component.DestroyImmediate(swapText.GetComponent<Image>());
            swapText.transform.localPosition = new Vector3(-180, -650, 0);
            swapText.name = "Chat Swap Text";
            appArchipelago.chatSwap = swapText.AddComponent<TextMeshProUGUI>();
            tmpfl = Traverse.Create(appArchipelago.chatSwap.gameObject.AddComponent<TMProFontLocalizer>());
            tmpfl.Field<GameFontType>("gameFontType").Value = Core.PhoneFont;
            tmpfl.Field<TextMeshProUGUI>("textMesh").Value = appArchipelago.chatSwap;
            appArchipelago.chatSwap.alignment = TextAlignmentOptions.Right;
            appArchipelago.chatSwap.fontSize = 70;
            appArchipelago.chatSwap.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_NAVIGATION_OPTIONS");

            appArchipelago.optionsBackground = GameObject.Instantiate(appArchipelago.chatBackground.gameObject, appArchipelago.Content);
            appArchipelago.optionsBackground.transform.Rotate(0, 0, 180);
            appArchipelago.optionsBackground.transform.localPosition = new Vector3(-90, -650, 0);
            appArchipelago.optionsBackground.name = "Options Bottom";
            appArchipelago.optionsBackground.SetActive(false);

            appArchipelago.arrowLeft = GameObject.Instantiate(appArchipelago.headerArrow, appArchipelago.Content);
            appArchipelago.arrowLeft.transform.localPosition = new Vector3(-440, -650, 0);
            appArchipelago.arrowLeft.name = "Arrow Left";
            appArchipelago.arrowLeft.SetActive(false);

            appArchipelago.optionsSwap = GameObject.Instantiate(appArchipelago.chatSwap.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.optionsSwap.alignment = TextAlignmentOptions.Left;
            appArchipelago.optionsSwap.transform.localPosition = new Vector3(200, -650, 0);
            appArchipelago.optionsSwap.text = "Chat";
            appArchipelago.optionsSwap.gameObject.name = "Options Swap Text";
            appArchipelago.optionsSwap.gameObject.SetActive(false);

            appArchipelago.optionCurrentText = GameObject.Instantiate(appArchipelago.text.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.optionCurrentText.alignment = TextAlignmentOptions.Left;
            appArchipelago.optionCurrentText.enableAutoSizing = true;
            appArchipelago.optionCurrentText.fontSizeMax = 90;
            appArchipelago.optionCurrentText.fontSizeMin = 70;
            appArchipelago.optionCurrentText.lineSpacing = 24;
            appArchipelago.optionCurrentText.transform.localPosition = new Vector3(0, 50, 0);
            Traverse.Create(appArchipelago.optionCurrentText.GetComponent<TMProFontLocalizer>()).Field<GameFontType>("gameFontType").Value = Core.PhoneFont;
            appArchipelago.optionCurrentText.GetComponent<RectTransform>().sizeDelta = new Vector2(900, 1560);
            appArchipelago.optionCurrentText.gameObject.name = "Current Option";
            appArchipelago.optionCurrentText.gameObject.SetActive(false);

            appArchipelago.optionPrevText = GameObject.Instantiate(appArchipelago.optionCurrentText.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.optionPrevText.alignment = TextAlignmentOptions.Capline;
            appArchipelago.optionPrevText.enableAutoSizing = false;
            appArchipelago.optionPrevText.fontSize = 60;
            appArchipelago.optionPrevText.color = PhoneBlue;
            appArchipelago.optionPrevText.transform.localPosition = new Vector3(0, 500, 0);
            appArchipelago.optionPrevText.gameObject.name = "Prev Option";
            appArchipelago.optionPrevText.gameObject.SetActive(false);

            appArchipelago.optionNextText = GameObject.Instantiate(appArchipelago.optionPrevText.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.optionNextText.alignment = TextAlignmentOptions.Bottom;
            appArchipelago.optionNextText.transform.localPosition = new Vector3(0, 350, 0);
            appArchipelago.optionNextText.gameObject.name = "Next Option";
            appArchipelago.optionNextText.gameObject.SetActive(false);

            appArchipelago.optionChangeText = GameObject.Instantiate(appArchipelago.chatSwap.gameObject, appArchipelago.Content).GetComponent<TextMeshProUGUI>();
            appArchipelago.optionChangeText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_NAVIGATION_CHANGE");
            appArchipelago.optionChangeText.color = PhoneOrange;
            appArchipelago.optionChangeText.fontSize = 65;
            appArchipelago.optionChangeText.gameObject.name = "Option Change Text";
            appArchipelago.optionChangeText.gameObject.SetActive(false);

            appArchipelago.optionChangeArrow = GameObject.Instantiate(appArchipelago.arrowRight.gameObject, appArchipelago.Content);
            appArchipelago.optionChangeArrow.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
            appArchipelago.optionChangeArrow.gameObject.name = "Option Change Arrow";
            appArchipelago.optionChangeArrow.gameObject.SetActive(false);

            appArchipelago.optionDownArrow = GameObject.Instantiate(Phone.GetAppInstance<AppHomeScreen>().Content.Find("BottomView").Find("OtherElements").Find("ArrowsContainer").Find("ArrowDown").gameObject, appArchipelago.Content);
            appArchipelago.optionDownArrow.transform.localPosition = new Vector3(0, -500, 0);
            appArchipelago.optionDownArrow.name = "Arrow Down";
            appArchipelago.optionDownArrow.gameObject.SetActive(false);

            appArchipelago.optionUpArrow = GameObject.Instantiate(appArchipelago.optionDownArrow, appArchipelago.Content);
            appArchipelago.optionUpArrow.transform.Rotate(0, 0, 180);
            appArchipelago.optionUpArrow.transform.localPosition = new Vector3(0, 600, 0);
            appArchipelago.optionUpArrow.name = "Arrow Up";
            appArchipelago.optionUpArrow.gameObject.SetActive(false);

            /*
            appEncounter = GameObject.Instantiate(appArchipelago.gameObject, appArchipelago.transform.parent).AddComponent<AppEncounter>();
            appEncounter.gameObject.name = "AppEncounter";
            Component.Destroy(appEncounter.GetComponent<AppArchipelago>());
            Traverse appET = Traverse.Create(appEncounter);
            appET.Method("Awake").GetValue();
            appET.Field<Phone>("<MyPhone>k__BackingField").Value = Phone;
            appET.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] { };
            appEncounter.Content.Find("Overlay").Find("Icons").Find("AppIcon").GetComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/encounter.png");
            appEncounter.headerText = appEncounter.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            appEncounter.headerText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_HEADER");
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
            */

            HomeScreenApp homeScreenAppEncounter = ScriptableObject.CreateInstance<HomeScreenApp>();
            homeScreenAppEncounter.name = "AppEncounter";
            Traverse hsaEncounter = Traverse.Create(homeScreenAppEncounter);
            hsaEncounter.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsaEncounter.Field<string>("m_AppName").Value = "AppEncounter";
            hsaEncounter.Field<string>("m_DisplayName").Value = "APP_ENCOUNTER_HEADER";
            hsaEncounter.Field<Sprite>("m_AppIcon").Value = UIManager.bundle.LoadAsset<Sprite>("assets/encounter.png");

            HomeScreenApp homeScreenAppArchipelago = ScriptableObject.CreateInstance<HomeScreenApp>();
            homeScreenAppArchipelago.name = "AppArchipelago";
            Traverse hsaArchipelago = Traverse.Create(homeScreenAppArchipelago);
            hsaArchipelago.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsaArchipelago.Field<string>("m_AppName").Value = "AppArchipelago";
            hsaArchipelago.Field<string>("m_DisplayName").Value = "APP_ARCHIPELAGO_HEADER_DEFAULT";
            hsaArchipelago.Field<Sprite>("m_AppIcon").Value = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");

            Traverse.Create(Phone).Field<Dictionary<string, App>>("<AppInstances>k__BackingField").Value.Add("AppEncounter", appEncounter);
            Traverse.Create(Phone.GetAppInstance<AppHomeScreen>()).Method("AddApp", new object[] { homeScreenAppEncounter }).GetValue();

            Traverse.Create(Phone).Field<Dictionary<string, App>>("<AppInstances>k__BackingField").Value.Add("AppArchipelago", appArchipelago);
            Traverse.Create(Phone.GetAppInstance<AppHomeScreen>()).Method("AddApp", new object[] { homeScreenAppArchipelago }).GetValue();

            GameObject mainGroup = appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value.transform.parent.gameObject;
            GameObject outsideGroup = appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value.transform.parent.gameObject;

            GameObject mainNotification = GameObject.Instantiate(mainGroup.transform.Find("GraffitiNotification").gameObject, mainGroup.transform);
            GameObject outsideNotification = GameObject.Instantiate(outsideGroup.transform.Find("GraffitiNotification").gameObject, outsideGroup.transform);

            mainNotification.GetComponentInChildren<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/notification.png");
            outsideNotification.transform.Find("ActionImage").GetComponent<Image>().sprite = UIManager.bundle.LoadAsset<Sprite>("assets/notification.png");

            appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value = mainNotification;
            appAT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value = outsideNotification;
            appAT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Main").Value = mainNotification.GetComponentInChildren<TextMeshProUGUI>();
            appAT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Outside").Value = outsideNotification.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
