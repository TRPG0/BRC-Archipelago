using HarmonyLib;
using Reptile;
using Reptile.Phone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Archipelago.Apps
{
    public class AppEncounter : App
    {
        public Encounter CurrentEncounter
        {
            get 
            {
                if (WorldHandler.instance == null) return null;
                else return whT.Field<Encounter>("currentEncounter").Value;
            }
        }

        private Traverse whT;

        public TextMeshProUGUI headerText;
        public TextMeshProUGUI topText;
        public TextMeshProUGUI centerText;
        public TextMeshProUGUI bottomLeftText;
        public UIButtonGlyphComponent bottomLeftGlyph;
        public TextMeshProUGUI bottomRightText;
        public UIButtonGlyphComponent bottomRightGlyph;

        public EncounterType CurrentType
        {
            get; private set;
        }

        public enum EncounterType
        {
            None,
            Combat,
            Combo,
            Dream,
            FloorIsLava,
            Race,
            Score,
            Unknown
        }

        public bool CanQuitCurrentEncounter => CurrentType == EncounterType.Race || CurrentType == EncounterType.Score;

        public void SetType()
        {
            if (CurrentEncounter == null) CurrentType = EncounterType.None;
            else if (CurrentEncounter is CombatEncounter) CurrentType = EncounterType.Combat;
            else if (CurrentEncounter is ComboEncounter) CurrentType = EncounterType.Combo;
            else if (CurrentEncounter is DreamEncounter) CurrentType = EncounterType.Dream;
            else if (CurrentEncounter is FloorIsLavaEncounter) CurrentType = EncounterType.FloorIsLava;
            else if (CurrentEncounter is RaceEncounter) CurrentType = EncounterType.Race;
            else if (CurrentEncounter is ScoreEncounter) CurrentType = EncounterType.Score;
            else CurrentType = EncounterType.Unknown;
        }

        public void Init()
        {
            whT = Traverse.Create(WorldHandler.instance);
            SetType();
            bottomRightText.text = Core.Instance.RandoLocalizer.GetRawTextValue("APP_ENCOUNTER_NAVIGATION_YES");
        }

        public void Start()
        {
            ResizeDeltas();
            Invoke("ResizeDeltas", 0.3f);
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            HandleInput = true;
            SetType();
            SetTopText();
            SetCenterText();
            SetBottomText();
            ShowAcceptInput(CanQuitCurrentEncounter);
        }

        public override void OnAppDisable()
        {
            base.OnAppDisable();
            HandleInput = false;
        }

        public override void OnReleaseLeft()
        {
            MyPhone.CloseCurrentApp();
        }

        public override void OnReleaseRight()
        {
            if (CanQuitCurrentEncounter)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Confirm);
                GiveUp();
            }
        }

        public void ResizeDeltas()
        {
            Vector2 statusTexts = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.89f);
            Vector2 glyphs = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.88f);
            Vector2 inputTexts = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.67f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.88f);
            topText.GetComponent<RectTransform>().sizeDelta = statusTexts;
            centerText.GetComponent<RectTransform>().sizeDelta = statusTexts;
            bottomLeftGlyph.GetComponent<RectTransform>().sizeDelta = glyphs;
            bottomRightGlyph.GetComponent<RectTransform>().sizeDelta = glyphs;
            bottomLeftText.GetComponent<RectTransform>().sizeDelta = inputTexts;
            bottomRightText.GetComponent<RectTransform>().sizeDelta = inputTexts;
        }

        public void ShowAcceptInput(bool show)
        {
            bottomRightText.gameObject.SetActive(show);
            bottomRightGlyph.gameObject.SetActive(show);
        }

        public void SetTopText()
        {
            topText.text = string.Format(Core.Instance.RandoLocalizer.GetRawTextValue("APP_ENCOUNTER_TYPE"), $"\n<b>{CurrentType}</b>");
        }

        public void SetCenterText()
        {
            if (CanQuitCurrentEncounter) centerText.text = Core.Instance.RandoLocalizer.GetRawTextValue("APP_ENCOUNTER_STATUS_CAN");
            else if (CurrentType == EncounterType.None) centerText.text = Core.Instance.RandoLocalizer.GetRawTextValue("APP_ENCOUNTER_STATUS_NONE");
            else centerText.text = Core.Instance.RandoLocalizer.GetRawTextValue("APP_ENCOUNTER_STATUS_CANNOT");
        }

        public void SetBottomText()
        {
            if (CanQuitCurrentEncounter) bottomLeftText.text = Core.Instance.RandoLocalizer.GetRawTextValue("APP_ENCOUNTER_NAVIGATION_NO");
            else bottomLeftText.text = Core.Instance.RandoLocalizer.GetRawTextValue("APP_NAVIGATION_CLOSE");
        }

        public void GiveUp()
        {
            if (!CanQuitCurrentEncounter) return;
            CurrentEncounter.SetEncounterState(Encounter.EncounterState.MAIN_EVENT_FAILED_DECAY);
            MyPhone.CloseCurrentApp();
            MyPhone.TurnOff();
        }
    }
}
