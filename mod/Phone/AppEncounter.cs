using HarmonyLib;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Reptile.Phone
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
            Score
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
        }

        public void Init()
        {
            whT = Traverse.Create(WorldHandler.instance);
            SetType();
            bottomRightText.text = "Yes";
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
                Traverse.Create(Core.Instance.AudioManager).Method("PlaySfxGameplay", new object[] { SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Confirm, 0f }).GetValue();
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
            topText.text = $"\n\nCurrent Encounter Type:\n<b>{CurrentType}</b>";
        }

        public void SetCenterText()
        {
            if (CanQuitCurrentEncounter) centerText.text = "\n\nAre you sure you want to give up on this encounter?";
            else if (CurrentType == EncounterType.None) centerText.text = "\n\nThere is no encounter active right now.";
            else centerText.text = "\n\nThis encounter cannot be ended early.";
        }

        public void SetBottomText()
        {
            if (CanQuitCurrentEncounter) bottomLeftText.text = "No";
            else bottomLeftText.text = "Close app";
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
