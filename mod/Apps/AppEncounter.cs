using HarmonyLib;
using ModLocalizer;
using Reptile;
using Reptile.Phone;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Archipelago.BRC.Apps
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
        public TextMeshProUGUI currentText;
        public TextMeshProUGUI messageText;
        public TextMeshProUGUI confirmText;
        public GameObject confirmArrow;

        public EncounterType CurrentType { get; private set; }

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
        }

        public void OnEnable()
        {
            SetType();
            SetCurrentText();
            SetMessageText();
            StartCoroutine(ShowAcceptInput(CanQuitCurrentEncounter));
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            HandleInput = true;
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

        public IEnumerator ShowAcceptInput(bool show)
        {
            if (show)
            {
                confirmText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_CONFIRM");
                yield return new WaitForEndOfFrame();
                float newY = messageText.transform.localPosition.y + (messageText.renderedHeight * 2) - 75;
                confirmText.transform.localPosition = new Vector3(-375, newY, 0);
                confirmText.gameObject.SetActive(true);
                yield return new WaitForEndOfFrame();
                confirmArrow.transform.localPosition = new Vector3(confirmText.renderedWidth + 25, newY, 0);
                confirmArrow.gameObject.SetActive(true);
            }
            else
            {
                confirmText.gameObject.SetActive(false);
                confirmArrow.gameObject.SetActive(false);
            }
        }

        public string CurrentTypeToString()
        {
            if (CurrentType == EncounterType.None) return Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_TYPE_NONE");
            else if (CurrentType == EncounterType.Unknown) return Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_TYPE_UNKNOWN");
            else return CurrentType.ToString();
        }

        public void SetCurrentText()
        {
            currentText.text = CurrentTypeToString();
        }

        public void SetMessageText()
        {
            if (CanQuitCurrentEncounter) messageText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_STATUS_CAN");
            else if (CurrentType == EncounterType.None) messageText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_STATUS_NONE");
            else messageText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ENCOUNTER_STATUS_CANNOT");
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
