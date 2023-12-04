using UnityEngine;
using Reptile;
using HarmonyLib;
using Archipelago.Components;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace Archipelago
{
    public class UIManager
    {
        public static AssetBundle bundle = AssetBundle.LoadFromMemory(Properties.Resources.archipelago);
        public static Color menuOrange = new(1, 0.54f, 0);

        public TextMeshProMenuButton SaveSlotOneButton;
        public TextMeshProMenuButton SaveSlotTwoButton;
        public TextMeshProMenuButton SaveSlotThreeButton;

        public APSlotButton APSlotOneButton;
        public APSlotButton APSlotTwoButton;
        public APSlotButton APSlotThreeButton;

        public List<APSlotButton> slotButtons = new List<APSlotButton>();

        public SaveSlotMenu ssm;
        public UIAnimationController uiac;
        public GameObject SelectionBarImage;

        public TextMeshProUGUI APMenuHeaderText;
        public TextMeshProUGUI APMenuNameLabel;
        public TextMeshProUGUI APMenuAddressLabel;
        public TextMeshProUGUI APMenuPasswordLabel;
        public TMP_InputField APMenuNameInput;
        public TMP_InputField APMenuAddressInput;
        public TMP_InputField APMenuPasswordInput;

        public TextMeshProUGUI APMenuStatus;
        public TextMeshProUGUI APMenuLocations;
        public const int totalLocations = 229;
        public TextMeshProUGUI APMenuResult;
        public TextMeshProUGUI APMenuChat;
        public int connectingSlot = -1;

        public SelectEnlargeButton APMenuConnectButton;
        public SelectEnlargeButton APMenuCancelButton;

        public static TMP_FontAsset font;

        public void FindSaveSlotMenu()
        {
            if (ssm != null) return;
            ssm = Traverse.Create(Reptile.Core.Instance.UIManager).Field<SaveSlotMenu>("saveSlotsMenu").Value;
        }

        public void FindUIAnimController()
        {
            if (uiac != null) return;
            uiac = Traverse.Create(ssm).Field<UIAnimationController[]>("menuAnimations").Value[0];
        }

        public void CreateAPButtons()
        {
            if (APSlotThreeButton != null) return;

            GameObject version = Traverse.Create(Reptile.Core.Instance.UIManager).Field<MainMenuManager>("mainMenu").Value.GetFirstFocusGameObject().transform.parent.Find("VersionText").gameObject;
            version.GetComponent<TextMeshProUGUI>().text = "ARCHIPELAGO: " + Core.PluginVersion + " (prerelease 3)\n" + version.GetComponent<TextMeshProUGUI>().text;
            version.GetComponent<TextMeshProUGUI>().verticalAlignment = VerticalAlignmentOptions.Bottom;

            FindSaveSlotMenu();
            FindUIAnimController();

            GameObject buttonsGroup = ssm.GetFirstFocusGameObject().transform.parent.gameObject;

            SaveSlotOneButton = buttonsGroup.transform.Find("SlotOneTextButton").GetComponent<TextMeshProMenuButton>();
            SaveSlotOneButton.onDelayedButtonClick.RemoveAllListeners();
            SaveSlotOneButton.onDelayedButtonClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            SaveSlotOneButton.onDelayedButtonClick.AddListener(delegate { CheckAndStartGameFromSlot(0); });

            SaveSlotTwoButton = buttonsGroup.transform.Find("SlotTwoTextButton").GetComponent<TextMeshProMenuButton>();
            SaveSlotTwoButton.onDelayedButtonClick.RemoveAllListeners();
            SaveSlotTwoButton.onDelayedButtonClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            SaveSlotTwoButton.onDelayedButtonClick.AddListener(delegate { CheckAndStartGameFromSlot(1); });

            SaveSlotThreeButton = buttonsGroup.transform.Find("SlotThreeTextButton").GetComponent<TextMeshProMenuButton>();
            SaveSlotThreeButton.onDelayedButtonClick.RemoveAllListeners();
            SaveSlotThreeButton.onDelayedButtonClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            SaveSlotThreeButton.onDelayedButtonClick.AddListener(delegate { CheckAndStartGameFromSlot(2); });

            //buttonsGroup.transform.Find("BackTextButton").GetComponent<TextMeshProMenuButton>().onClick.AddListener(delegate { HideMenu(); APMenuChat.gameObject.SetActive(false); });


            APSlotOneButton = GameObject.Instantiate(buttonsGroup.transform.Find("DeleteSlotOneButton").gameObject, buttonsGroup.transform).AddComponent<APSlotButton>();
            slotButtons.Add(APSlotOneButton);

            APSlotTwoButton = GameObject.Instantiate(buttonsGroup.transform.Find("DeleteSlotTwoButton").gameObject, buttonsGroup.transform).AddComponent<APSlotButton>();
            slotButtons.Add(APSlotTwoButton);

            APSlotThreeButton = GameObject.Instantiate(buttonsGroup.transform.Find("DeleteSlotThreeButton").gameObject, buttonsGroup.transform).AddComponent<APSlotButton>();
            slotButtons.Add(APSlotThreeButton);

            float xpos1 = buttonsGroup.transform.Find("DeleteSlotOneButton").localPosition.x;
            float width = buttonsGroup.transform.Find("DeleteSlotOneButton").GetComponent<RectTransform>().rect.width;
            float xpos2 = xpos1 - (width * 2);

            Vector3 pos1 = APSlotOneButton.transform.localPosition;
            pos1.x = xpos2;
            APSlotOneButton.transform.localPosition = pos1;

            Vector3 pos2 = APSlotTwoButton.transform.localPosition;
            pos2.x = xpos2;
            APSlotTwoButton.transform.localPosition = pos2;

            Vector3 pos3 = APSlotThreeButton.transform.localPosition;
            pos3.x = xpos2;
            APSlotThreeButton.transform.localPosition = pos3;

            APSlotOneButton.Init();
            APSlotTwoButton.Init();
            APSlotThreeButton.Init();

            GameObject origImg = uiac.transform.Find("SelectionBarImageSlideIn").GetComponent<DOTweenAnimation>().targetGO;

            SelectionBarImage = GameObject.Instantiate(origImg, origImg.transform.parent);
            Vector3 pos4 = SelectionBarImage.transform.localPosition;
            float ydiff = SelectionBarImage.GetComponent<RectTransform>().rect.height + (float)(SelectionBarImage.GetComponent<RectTransform>().rect.height * 0.19);
            pos4.y = pos4.y + ydiff;
            SelectionBarImage.transform.localPosition = pos4;
            SelectionBarImage.transform.SetSiblingIndex(1);

            APMenuHeaderText = GameObject.Instantiate(buttonsGroup.transform.Find("MenuSubHeaderText"), buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuHeaderText.GetComponent<TMProFontLocalizer>().UpdateTextMeshLanguageFont(SystemLanguage.English);
            font = APMenuHeaderText.font;
            Component.Destroy(APMenuHeaderText.GetComponent<TMProLocalizationAddOn>());
            Component.Destroy(APMenuHeaderText.GetComponent<TMProFontLocalizer>());
            Color color = APMenuHeaderText.color;
            color.a = 1;
            APMenuHeaderText.color = color;
            APMenuHeaderText.text = "Archipelago / Slate ?";
            Vector3 pos5 = APMenuHeaderText.transform.localPosition;
            pos5.y = pos5.y + ydiff;
            APMenuHeaderText.transform.localPosition = pos5;

            APMenuNameLabel = GameObject.Instantiate(APMenuHeaderText.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuNameLabel.fontStyle = FontStyles.LowerCase;
            APMenuNameLabel.color = menuOrange;
            APMenuNameLabel.text = "Name";
            Vector3 pos6 = APMenuHeaderText.transform.localPosition;
            pos6.x = pos6.x + (APMenuNameLabel.GetComponent<RectTransform>().rect.height * 1.8f);
            pos6.y = pos6.y - (APMenuNameLabel.GetComponent<RectTransform>().rect.height * 1.3f);
            APMenuNameLabel.transform.localPosition = pos6;

            APMenuAddressLabel = GameObject.Instantiate(APMenuNameLabel.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuAddressLabel.text = "Address";
            Vector3 pos7 = APMenuNameLabel.transform.localPosition;
            pos7.y = pos7.y - (APMenuAddressLabel.GetComponent<RectTransform>().rect.height * 1.3f);
            APMenuAddressLabel.transform.localPosition = pos7;

            APMenuPasswordLabel = GameObject.Instantiate(APMenuAddressLabel.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuPasswordLabel.text = "Password";
            Vector3 pos8 = APMenuAddressLabel.transform.localPosition;
            pos8.y = pos8.y - (APMenuPasswordLabel.GetComponent<RectTransform>().rect.height * 1.3f);
            APMenuPasswordLabel.transform.localPosition = pos8;

            APMenuNameInput = GameObject.Instantiate(APMenuNameLabel.gameObject, buttonsGroup.transform).AddComponent<TMP_InputField>();
            APMenuNameInput.textComponent = APMenuNameInput.GetComponent<TextMeshProUGUI>();
            APMenuNameInput.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Underline;
            APMenuNameInput.GetComponent<TextMeshProUGUI>().color = Color.black;
            APMenuNameInput.text = "Red";
            Vector3 pos9 = APMenuNameLabel.transform.localPosition;
            pos9.x = pos9.x + (APMenuNameInput.GetComponent<RectTransform>().rect.height * 3);
            APMenuNameInput.transform.localPosition = pos9;

            APMenuAddressInput = GameObject.Instantiate(APMenuNameInput.gameObject, buttonsGroup.transform).GetComponent<TMP_InputField>();
            APMenuAddressInput.text = "archipelago.gg";
            Vector3 pos10 = APMenuNameInput.transform.localPosition;
            pos10.y = APMenuAddressLabel.transform.localPosition.y;
            APMenuAddressInput.transform.localPosition = pos10;

            APMenuPasswordInput = GameObject.Instantiate(APMenuAddressInput.gameObject, buttonsGroup.transform).GetComponent<TMP_InputField>();
            APMenuPasswordInput.text = string.Empty;
            Vector3 pos11 = APMenuNameInput.transform.localPosition;
            pos11.y = APMenuPasswordLabel.transform.localPosition.y;
            APMenuPasswordInput.transform.localPosition = pos11;

            APMenuResult = GameObject.Instantiate(APMenuPasswordLabel.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuResult.fontStyle = FontStyles.Normal;
            APMenuResult.color = Color.black;
            APMenuResult.text = string.Empty;
            Vector3 pos12 = APMenuPasswordLabel.transform.localPosition;
            pos12.y = pos12.y - (APMenuResult.GetComponent<RectTransform>().rect.height * 1.3f);
            APMenuResult.transform.localPosition = pos12;

            APMenuStatus = GameObject.Instantiate(APMenuNameLabel.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuStatus.horizontalAlignment = HorizontalAlignmentOptions.Right;
            APMenuStatus.fontStyle = FontStyles.Normal;
            APMenuStatus.color = Color.black;
            APMenuStatus.text = "Status: Not connected.";
            Vector3 pos13 = APMenuNameLabel.transform.localPosition;
            pos13.x = pos13.x - (APMenuStatus.GetComponent<RectTransform>().rect.height * 2.2f);
            APMenuStatus.transform.localPosition = pos13;
            APMenuStatus.transform.SetSiblingIndex(15);

            APMenuLocations = GameObject.Instantiate(APMenuAddressLabel.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuLocations.horizontalAlignment = HorizontalAlignmentOptions.Right;
            APMenuLocations.fontStyle = FontStyles.Normal;
            APMenuLocations.color = Color.black;
            APMenuLocations.text = $"? / {totalLocations}";
            Vector3 pos14 = APMenuStatus.transform.localPosition;
            pos14.y = APMenuAddressLabel.transform.localPosition.y;
            APMenuLocations.transform.localPosition = pos14;
            APMenuLocations.transform.SetSiblingIndex(16);

            APMenuChat = GameObject.Instantiate(APMenuNameLabel.gameObject, buttonsGroup.transform).GetComponent<TextMeshProUGUI>();
            APMenuChat.fontSize = APMenuNameLabel.fontSize;
            APMenuChat.fontStyle = FontStyles.Normal;
            APMenuChat.alignment = TextAlignmentOptions.BottomLeft;
            //APMenuChat.color = new Color(0.875f, 0.871f, 0.753f);
            APMenuChat.color = new Color(0.925f, 0.91f, 0.796f);
            APMenuChat.transform.localPosition = new Vector3(-612, -240, 0);
            APMenuChat.gameObject.AddComponent<ResizeChatOnEnable>().parentTransform = APMenuChat.GetComponent<RectTransform>();
            APMenuChat.GetComponent<ResizeChatOnEnable>().targetTransform = buttonsGroup.transform.parent.Find("SwirlBottom").GetComponent<RectTransform>();
            APMenuChat.gameObject.SetActive(false);

            APMenuCancelButton = GameObject.Instantiate(buttonsGroup.transform.Find("DeleteSlotThreeButton").gameObject, buttonsGroup.transform).GetComponent<SelectEnlargeButton>();
            Vector3 pos15 = buttonsGroup.transform.Find("DeleteSlotThreeButton").localPosition;
            pos15.y = pos15.y + ydiff;
            APMenuCancelButton.transform.localPosition = pos15;
            APMenuCancelButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            APMenuCancelButton.onClick.RemoveAllListeners();
            APMenuCancelButton.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            APMenuCancelButton.onClick.AddListener(Core.Instance.UIManager.HideMenu);

            APMenuConnectButton = GameObject.Instantiate(APSlotThreeButton.gameObject, buttonsGroup.transform).GetComponent<SelectEnlargeButton>();
            Vector3 pos16 = APSlotThreeButton.transform.localPosition;
            pos16.y = pos16.y + ydiff;
            APMenuConnectButton.transform.localPosition = pos16;
            Component.Destroy(APMenuConnectButton.GetComponent<APSlotButton>());
            APMenuConnectButton.GetComponent<Image>().sprite = bundle.LoadAsset<Sprite>("assets/check1.png");
            SpriteState spriteState = new SpriteState
            {
                highlightedSprite = bundle.LoadAsset<Sprite>("assets/check2.png"),
                selectedSprite = bundle.LoadAsset<Sprite>("assets/check2.png")
            };
            APMenuConnectButton.GetComponent<SelectEnlargeButton>().spriteState = spriteState;
            APMenuConnectButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            APMenuConnectButton.onClick.RemoveAllListeners();
            APMenuConnectButton.onClick.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            APMenuConnectButton.onClick.AddListener(delegate { Core.Instance.Multiworld.Connect(connectingSlot, APMenuNameInput.text, APMenuAddressInput.text, APMenuPasswordInput.text); });

            HideMenu();
        }

        public void HideMenu()
        {
            if (APMenuConnectButton == null) return;

            SelectionBarImage.SetActive(false);
            APMenuHeaderText.gameObject.SetActive(false);
            APMenuNameLabel.gameObject.SetActive(false);
            APMenuAddressLabel.gameObject.SetActive(false);
            APMenuPasswordLabel.gameObject.SetActive(false);
            APMenuNameInput.gameObject.SetActive(false);
            APMenuAddressInput.gameObject.SetActive(false);
            APMenuPasswordInput.gameObject.SetActive(false);
            APMenuStatus.gameObject.SetActive(false);
            APMenuLocations.gameObject.SetActive(false);
            APMenuResult.gameObject.SetActive(false);
            APMenuConnectButton.gameObject.SetActive(false);
            APMenuCancelButton.gameObject.SetActive(false);
        }

        public void ShowMenu(APSlotButton APButton)
        {
            if (APMenuConnectButton == null) return;

            AudioManager am = Traverse.Create(APButton.button).Field<AudioManager>("audioManager").Value;

            if (APButton.CurrentState == APSlotButton.SlotState.Vanilla || (Core.Instance.Multiworld.Authenticated && APButton.slot != Core.Instance.SaveManager.currentSlot))
            {
                PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                return;
            }

            PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm);
            connectingSlot = APButton.slot;
            SetHeaderSlot(APButton.slot);

            Core.Instance.SaveManager.LoadData(connectingSlot);
            SetStatus(APButton.CurrentState);
            if (Core.Instance.Data.slot_name != null) APMenuNameInput.text = Core.Instance.Data.slot_name;
            else APMenuNameInput.text = Core.configDefaultName.Value;
            if (Core.Instance.Data.host_name != null) APMenuAddressInput.text = Core.Instance.Data.host_name;
            else APMenuAddressInput.text = Core.configDefaultAddress.Value;
            if (Core.Instance.Data.password != null) APMenuPasswordInput.text = Core.Instance.Data.password;
            else APMenuPasswordInput.text = Core.configDefaultPassword.Value;

            SelectionBarImage.SetActive(true);
            APMenuHeaderText.gameObject.SetActive(true);
            APMenuNameLabel.gameObject.SetActive(true);
            APMenuAddressLabel.gameObject.SetActive(true);
            APMenuPasswordLabel.gameObject.SetActive(true);
            APMenuNameInput.gameObject.SetActive(true);
            APMenuAddressInput.gameObject.SetActive(true);
            APMenuPasswordInput.gameObject.SetActive(true);
            APMenuStatus.gameObject.SetActive(true);
            APMenuLocations.gameObject.SetActive(true);
            APMenuResult.gameObject.SetActive(true);
            APMenuConnectButton.gameObject.SetActive(true);
            APMenuCancelButton.gameObject.SetActive(true);
        }

        public void SetHeaderSlot(int num)
        {
            if (APMenuHeaderText == null) return;
            APMenuHeaderText.text = $"Archipelago / Slate {num+1}";
        }


        public void SetStatus(APSlotButton.SlotState state)
        {
            string status = string.Empty;
            string locations = Core.Instance.SaveManager.DataExists(connectingSlot) ? Core.Instance.Data.@checked.Count.ToString() : "?";

            switch (state)
            {
                default:
                case APSlotButton.SlotState.Vanilla:
                    status = "Slate is not randomized.";
                    break;
                case APSlotButton.SlotState.NoData:
                    status = "Status: No data.";
                    break;
                case APSlotButton.SlotState.YesData:
                    status = "Status: Not connected.";
                    break;
                case APSlotButton.SlotState.Connected:
                    status = "Status: Connected.";
                    break;
                case APSlotButton.SlotState.Disconnected:
                    status = "Status: Connection failed.";
                    break;
            }

            APMenuStatus.text = status;
            APMenuLocations.text = $"{locations} / {totalLocations}";
        }

        public void SetResult(string result)
        {
            APMenuResult.text = result;
        }

        public void CheckSlots()
        {
            Reptile.SaveManager sm = Traverse.Create(ssm).Field<Reptile.SaveManager>("saveManager").Value;

            if (!sm.IsSaveSlotOccupied(0)) APSlotOneButton.ChangeState(APSlotButton.SlotState.NoData);
            else
            {
                if (File.Exists(Path.Combine(Core.Instance.SaveManager.FolderPath, "archipelago0.json"))) APSlotOneButton.ChangeState(APSlotButton.SlotState.YesData);
                else APSlotOneButton.ChangeState(APSlotButton.SlotState.Vanilla);
            }

            if (!sm.IsSaveSlotOccupied(1)) APSlotTwoButton.ChangeState(APSlotButton.SlotState.NoData);
            else
            {
                if (File.Exists(Path.Combine(Core.Instance.SaveManager.FolderPath, "archipelago1.json"))) APSlotTwoButton.ChangeState(APSlotButton.SlotState.YesData);
                else APSlotTwoButton.ChangeState(APSlotButton.SlotState.Vanilla);
            }

            if (!sm.IsSaveSlotOccupied(2)) APSlotThreeButton.ChangeState(APSlotButton.SlotState.NoData);
            else
            {
                if (File.Exists(Path.Combine(Core.Instance.SaveManager.FolderPath, "archipelago2.json"))) APSlotThreeButton.ChangeState(APSlotButton.SlotState.YesData);
                else APSlotThreeButton.ChangeState(APSlotButton.SlotState.Vanilla);
            }
        }

        public void CheckAndStartGameFromSlot(int slotId)
        {
            if (File.Exists(Path.Combine(Core.Instance.SaveManager.FolderPath, string.Format("archipelago{0}.json", slotId))))
            {
                if (!Core.Instance.Multiworld.Authenticated)
                {
                    Core.Logger.LogInfo($"Save slot {slotId} has randomizer data, but is not connected.");
                    PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                }
                else if (slotId != Core.Instance.SaveManager.currentSlot)
                {
                    Core.Logger.LogInfo($"Save slot {slotId} does not match connected slot. ({Core.Instance.SaveManager.currentSlot})");
                    PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                }
                else
                {
                    Core.Logger.LogInfo($"Save slot {slotId} has randomizer data and is connected. Starting game.");
                    Traverse.Create(ssm).Method("StartGameFromSlot", new object[] { slotId }).GetValue();
                    HideMenu();
                    APMenuChat.gameObject.SetActive(false);
                }
            }
            else
            {
                if (Core.Instance.Multiworld.Authenticated)
                {
                    Core.Logger.LogInfo($"Save slot {slotId} has no randomizer data, but is currently connected.");
                    PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.cancel);
                }
                else
                {
                    Core.Logger.LogInfo($"Save slot {slotId} has no randomizer data and is not connected. Starting game.");
                    Traverse.Create(ssm).Method("StartGameFromSlot", new object[] { slotId }).GetValue();
                    HideMenu();
                    APMenuChat.gameObject.SetActive(false);
                }
            }
        }

        public void PlaySfxUI(SfxCollectionID collectionId, AudioClipID audioClipId, float randomPitchVariance = 0f)
        {
            Traverse.Create(Reptile.Core.Instance.AudioManager).Method("PlaySfxUI", new object[] { collectionId, audioClipId, randomPitchVariance }).GetValue();
        }

        public void PlaySfxGameplay(SfxCollectionID collectionId, AudioClipID audioClipId, float randomPitchVariance = 0f)
        {
            Traverse.Create(Reptile.Core.Instance.AudioManager).Method("PlaySfxGameplay", new object[] { collectionId, audioClipId, randomPitchVariance }).GetValue();
        }
    }
}
