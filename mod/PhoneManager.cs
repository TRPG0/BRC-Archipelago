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

        public AppArchipelago app;

        public void DoAppSetup()
        {
            if (!Reptile.Core.Instance.BaseModule.IsPlayingInStage) return;
            if (app != null) return;

            app = GameObject.Instantiate(Phone.GetComponentInChildren<AppEmail>(true).transform.gameObject, Phone.GetComponentInChildren<AppEmail>(true).transform.parent).AddComponent<AppArchipelago>();
            app.gameObject.name = "AppArchipelago";
            Component.Destroy(app.GetComponent<AppEmail>());
            Traverse appT = Traverse.Create(app);
            appT.Method("Awake").GetValue();
            appT.Field<Phone>("<MyPhone>k__BackingField").Value = Phone;
            appT.Field<AUnlockable[]>("m_Unlockables").Value = new AUnlockable[] {};
            //appT.Field<Notification>("m_Notification").Value = null;

            GameObject.Destroy(app.Content.Find("EmailScroll").gameObject);
            GameObject.Destroy(app.Content.Find("MessagePanel").gameObject);

            app.Content.Find("Overlay").GetComponentInChildren<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            Component.Destroy(app.Content.Find("Overlay").GetComponentInChildren<TMProFontLocalizer>());
            Component.Destroy(app.Content.Find("Overlay").GetComponentInChildren<TMProLocalizationAddOn>());
            TextMeshProUGUI headerText = app.Content.Find("Overlay").GetComponentInChildren<TextMeshProUGUI>();
            headerText.text = "Archipelago";

            Image icon = app.Content.Find("Overlay").Find("Icons").Find("AppIcon").GetComponentInChildren<Image>();
            icon.sprite = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");

            GameObject textObject = new GameObject()
            {
                name = "Messages",
                layer = 24,
            };
            textObject.transform.parent = app.Content.transform;
            textObject.transform.localPosition = Vector3.zero;
            textObject.transform.SetAsFirstSibling();
            app.text = textObject.AddComponent<TextMeshProUGUI>();
            app.UpdateText();
            app.text.fontSize = 24;
            app.text.font = UIManager.font;
            app.text.verticalAlignment = VerticalAlignmentOptions.Bottom;
            app.text.enableWordWrapping = true;
            app.GetComponent<RectTransform>().sizeDelta = new Vector2(app.transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.95f, app.transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.9f);

            HomeScreenApp homeScreenApp = ScriptableObject.CreateInstance<HomeScreenApp>();
            homeScreenApp.name = "AppArchipelago";
            Traverse hsa = Traverse.Create(homeScreenApp);
            hsa.Field<HomeScreenApp.HomeScreenAppType>("appType").Value = HomeScreenApp.HomeScreenAppType.EMAIL;
            hsa.Field<string>("m_AppName").Value = "AppArchipelago";
            hsa.Field<string>("m_DisplayName").Value = "ARCHIPELAGO";
            hsa.Field<Sprite>("m_AppIcon").Value = UIManager.bundle.LoadAsset<Sprite>("assets/archipelago.png");
            Traverse.Create(Phone).Field<Dictionary<string, App>>("<AppInstances>k__BackingField").Value.Add("AppArchipelago", app);
            Traverse.Create(Phone.GetAppInstance<AppHomeScreen>()).Method("AddApp", new object[] { homeScreenApp }).GetValue();

            GameObject mainGroup = appT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value.transform.parent.gameObject;
            GameObject outsideGroup = appT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value.transform.parent.gameObject;

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

            appT.Property("Notification").Field<GameObject>("appNotificationPanel_Main").Value = mainNotification;
            appT.Property("Notification").Field<GameObject>("appNotificationPanel_Outside").Value = outsideNotification;
            appT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Main").Value = mainNotification.GetComponentInChildren<TextMeshProUGUI>();
            appT.Property("Notification").Field<TextMeshProUGUI>("topBarText_Outside").Value = outsideNotification.GetComponentInChildren<TextMeshProUGUI>();
        }
    }
}
