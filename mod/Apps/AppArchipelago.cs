using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Archipelago.BRC.Structures;
using ModLocalizer;
using System.Collections;

namespace Archipelago.BRC.Apps
{
    public class AppArchipelago : App
    {
        public enum AppArchipelagoState
        {
            Chat,
            Options
        }

        public enum AppArchipelagoOptions
        {
            Logic,
            SkipIntro,
            SkipDreams,
            SkipHands,
            TotalREP,
            EndingREP,
            StartingMovestyle,
            LimitedGraffiti,
            SGraffiti,
            JunkPhotos,
            DontSavePhotos,
            ScoreDifficulty,
            DamageMultiplier,
            DeathLink
        }

        public List<string> Messages => Multiworld.messages;

        public AppArchipelagoState State { get; private set; } = AppArchipelagoState.Chat;
        public AppArchipelagoOptions CurrentOption { get; private set; } = 0;

        public static readonly List<AppArchipelagoOptions> canChange = new List<AppArchipelagoOptions>()
        {
            AppArchipelagoOptions.DontSavePhotos,
            AppArchipelagoOptions.ScoreDifficulty,
            AppArchipelagoOptions.DamageMultiplier,
            AppArchipelagoOptions.DeathLink
        };

        public TextMeshProUGUI title;
        public GameObject headerArrow;
        public TextMeshProUGUI text;
        public TextMeshProUGUI chatSwap;
        public GameObject chatBackground;
        public GameObject arrowRight;
        public TextMeshProUGUI optionsSwap;
        public GameObject optionsBackground;
        public GameObject arrowLeft;
        public TextMeshProUGUI optionCurrentText;
        public TextMeshProUGUI optionPrevText;
        public TextMeshProUGUI optionNextText;
        public TextMeshProUGUI optionChangeText;
        public GameObject optionChangeArrow;
        public GameObject optionUpArrow;
        public GameObject optionDownArrow;

        public static Color fadeColor = new Color(0.192f, 0.35f, 0.647f);
        public static Color highlightColor = new Color(0.882f, 0.953f, 0.345f);

        public void UpdateText()
        {
            text.text = string.Join("\n", Messages.ToArray());
        }

        public AppArchipelagoOptions PrevOption
        {
            get
            {
                AppArchipelagoOptions option = CurrentOption - 1;
                if (option == AppArchipelagoOptions.SGraffiti && !Core.Instance.Data.limitedGraffiti) option--;
                if (option < AppArchipelagoOptions.Logic) option = (AppArchipelagoOptions)((int)AppArchipelagoOptions.DeathLink + 1 + (int)option);
                return option;
            }
        }

        public AppArchipelagoOptions NextOption
        {
            get
            {
                AppArchipelagoOptions option = CurrentOption + 1;
                if (option == AppArchipelagoOptions.SGraffiti && !Core.Instance.Data.limitedGraffiti) option++;
                if (option > AppArchipelagoOptions.DeathLink) option = (AppArchipelagoOptions)((int)option - ((int)AppArchipelagoOptions.DeathLink + 1));
                return option;
            }
        }

        internal IEnumerator UpdateOptionText()
        {
            optionCurrentText.text = $"{GetOptionNameText(CurrentOption)}:\n<align=\"right\"><color=#{ColorUtility.ToHtmlStringRGBA(PhoneManager.PhoneYellow)}>{GetOptionValueText(CurrentOption)}</color></align>";
            optionPrevText.text = GetOptionNameText(PrevOption);
            optionNextText.text = GetOptionNameText(NextOption);

            if (canChange.Contains(CurrentOption))
            {
                optionChangeText.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_NAVIGATION_CHANGE");
                yield return new WaitForEndOfFrame();
                float newY = optionCurrentText.transform.localPosition.y - (optionCurrentText.renderedHeight / 2) - 75;
                optionChangeText.transform.localPosition = new Vector3(-165, newY, 0);
                optionChangeArrow.transform.localPosition = new Vector3(450, newY, 0);
                optionChangeText.gameObject.SetActive(true);
                optionChangeArrow.SetActive(true);
            }
            else
            {
                optionChangeText.gameObject.SetActive(false);
                optionChangeArrow.SetActive(false);
            }
            yield return null;
        }

        private string GetOptionNameText(AppArchipelagoOptions option)
        {
            return option switch
            {
                AppArchipelagoOptions.Logic => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_LOGIC"),
                AppArchipelagoOptions.SkipIntro => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_SKIPINTRO"),
                AppArchipelagoOptions.SkipDreams => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_SKIPDREAMS"),
                AppArchipelagoOptions.SkipHands => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_SKIPHANDS"),
                AppArchipelagoOptions.TotalREP => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_TOTALREP"),
                AppArchipelagoOptions.EndingREP => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_ENDINGREP"),
                AppArchipelagoOptions.StartingMovestyle => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_STARTINGMOVESTYLE"),
                AppArchipelagoOptions.LimitedGraffiti => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_LIMITEDGRAFFITI"),
                AppArchipelagoOptions.SGraffiti => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_SGRAFFITI"),
                AppArchipelagoOptions.JunkPhotos => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_JUNKPHOTOS"),
                AppArchipelagoOptions.DontSavePhotos => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_DONTSAVEPHOTOS"),
                AppArchipelagoOptions.ScoreDifficulty => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_SCOREDIFFICULTY"),
                AppArchipelagoOptions.DamageMultiplier => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_DAMAGEMULTIPLIER"),
                AppArchipelagoOptions.DeathLink => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "OPTION_DEATHLINK"),
                _ => "?"
            };
        }

        private string GetOptionValueText(AppArchipelagoOptions option)
        {
            return option switch
            {
                AppArchipelagoOptions.Logic => ValueToText(Core.Instance.Data.logic),
                AppArchipelagoOptions.SkipIntro => ValueToText(Core.Instance.Data.skipIntro),
                AppArchipelagoOptions.SkipDreams => ValueToText(Core.Instance.Data.skipDreams),
                AppArchipelagoOptions.SkipHands => ValueToText(Core.Instance.Data.skipHands),
                AppArchipelagoOptions.TotalREP => ValueToText(Core.Instance.Data.totalRep),
                AppArchipelagoOptions.EndingREP => ValueToText(Core.Instance.Data.endingRep),
                AppArchipelagoOptions.StartingMovestyle => ValueToText(Core.Instance.Data.startingMovestyle),
                AppArchipelagoOptions.LimitedGraffiti => ValueToText(Core.Instance.Data.limitedGraffiti),
                AppArchipelagoOptions.SGraffiti => ValueToText(Core.Instance.Data.sGraffiti),
                AppArchipelagoOptions.JunkPhotos => ValueToText(Core.Instance.Data.junkPhotos),
                AppArchipelagoOptions.DontSavePhotos => ValueToText(Core.configDontSavePhotos.Value),
                AppArchipelagoOptions.ScoreDifficulty => ValueToText(Core.Instance.Data.scoreDifficulty),
                AppArchipelagoOptions.DamageMultiplier => ValueToText(Core.Instance.Data.damageMultiplier),
                AppArchipelagoOptions.DeathLink => ValueToText(Core.Instance.Data.deathLink),
                _ => "?"
            };
        }

        private string ValueToText(object value)
        {
            if (value is bool boolValue)
            {
                if (boolValue) return Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_BOOL_TRUE");
                else return Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_BOOL_FALSE");
            }
            else if (value is Logic logicValue)
            {
                return logicValue switch
                {
                    Logic.Glitchless => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_LOGIC_GLITCHLESS"),
                    Logic.Glitched => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_LOGIC_GLITCHED"),
                    _ => "?"
                };
            }
            else if (value is MoveStyle styleValue)
            {
                return styleValue switch
                {
                    MoveStyle.SKATEBOARD => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_MOVESTYLE_SKATEBOARD"),
                    MoveStyle.INLINE => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_MOVESTYLE_INLINE"),
                    MoveStyle.BMX => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_MOVESTYLE_BMX"),
                    _ => "?"
                };
            }
            else if (value is SGraffiti sgraffitiValue)
            {
                return sgraffitiValue switch
                {
                    SGraffiti.Separate => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SGRAFFITI_SEPARATE"),
                    SGraffiti.Combined => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SGRAFFITI_COMBINED"),
                    _ => "?"
                };
            }
            else if (value is ScoreDifficulty scoreValue)
            {
                return scoreValue switch
                {
                    ScoreDifficulty.Normal => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SCOREDIFFICULTY_NORMAL"),
                    ScoreDifficulty.Medium => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SCOREDIFFICULTY_MEDIUM"),
                    ScoreDifficulty.Hard => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SCOREDIFFICULTY_HARD"),
                    ScoreDifficulty.VeryHard => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SCOREDIFFICULTY_VERYHARD"),
                    ScoreDifficulty.Extreme => Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "VALUE_SCOREDIFFICULTY_EXTREME"),
                    _ => "?"
                };
            }
            else if (value is int) return value.ToString();
            else return "?";
        }

        internal void UpdateHeader()
        {
            if (State == AppArchipelagoState.Chat) title.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_HEADER_DEFAULT");
            else if (State == AppArchipelagoState.Options) title.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_HEADER_OPTIONS");
        }

        public void ChangeState(AppArchipelagoState state)
        {
            if (state == AppArchipelagoState.Chat)
            {
                State = AppArchipelagoState.Chat;
                UpdateHeader();
                headerArrow.SetActive(true);
                text.gameObject.SetActive(true);
                chatSwap.gameObject.SetActive(true);
                chatBackground.SetActive(true);
                arrowRight.SetActive(true);
                optionsSwap.gameObject.SetActive(false);
                optionsBackground.SetActive(false);
                arrowLeft.SetActive(false);
                optionCurrentText.gameObject.SetActive(false);
                optionPrevText.gameObject.SetActive(false);
                optionNextText.gameObject.SetActive(false);
                optionChangeText.gameObject.SetActive(false);
                optionChangeArrow.SetActive(false);
                optionUpArrow.SetActive(false);
                optionDownArrow.SetActive(false);
            }
            else if (state == AppArchipelagoState.Options)
            {
                State = AppArchipelagoState.Options;
                UpdateHeader();
                headerArrow.SetActive(false);
                StartCoroutine(UpdateOptionText());
                text.gameObject.SetActive(false);
                chatSwap.gameObject.SetActive(false);
                chatBackground.SetActive(false);
                arrowRight.SetActive(false);
                optionsSwap.gameObject.SetActive(true);
                optionsBackground.SetActive(true);
                arrowLeft.SetActive(true);
                optionCurrentText.gameObject.SetActive(true);
                optionPrevText.gameObject.SetActive(true);
                optionNextText.gameObject.SetActive(true);
                optionUpArrow.SetActive(true);
                optionDownArrow.SetActive(true);
            }
        }

        public override void OnAppRefresh()
        {
            base.OnAppRefresh();
            UpdateText();
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            HandleInput = true;
            UpdateText();
        }

        public override void OnAppDisable()
        {
            base.OnAppDisable();
            HandleInput = false;
        }

        public void OnEnable()
        {
            UpdateHeader();
            chatSwap.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_NAVIGATION_OPTIONS");
            optionsSwap.text = Core.Instance.Localizer.GetRawTextValue(Subgroups.Text, "APP_ARCHIPELAGO_NAVIGATION_CHAT");
            if (State == AppArchipelagoState.Options) StartCoroutine(UpdateOptionText());
        }

        public override void OnReleaseLeft()
        {
            if (State == AppArchipelagoState.Chat) MyPhone.CloseCurrentApp();
            else
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                ChangeState(AppArchipelagoState.Chat);
            }
        }

        public override void OnReleaseRight()
        {
            if (State == AppArchipelagoState.Chat)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                ChangeState(AppArchipelagoState.Options);
            }
            else if (State == AppArchipelagoState.Options)
            {
                if (CurrentOption == AppArchipelagoOptions.DontSavePhotos)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.configDontSavePhotos.Value ^= true;
                    StartCoroutine(UpdateOptionText());
                }
                else if (CurrentOption == AppArchipelagoOptions.ScoreDifficulty)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.Instance.Data.scoreDifficulty += 1;
                    if (Core.Instance.Data.scoreDifficulty > ScoreDifficulty.Extreme) Core.Instance.Data.scoreDifficulty = ScoreDifficulty.Normal;
                    Core.Instance.stageManager.SetEncounterScores(WorldHandler.instance.SceneObjectsRegister);
                    StartCoroutine(UpdateOptionText());
                }
                else if (CurrentOption == AppArchipelagoOptions.DamageMultiplier)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.Instance.Data.damageMultiplier += 1;
                    if (Core.Instance.Data.damageMultiplier > 6) Core.Instance.Data.damageMultiplier = 1;
                    StartCoroutine(UpdateOptionText());
                }
                else if (CurrentOption == AppArchipelagoOptions.DeathLink)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.Instance.Data.deathLink ^= true;
                    if (Core.Instance.Data.deathLink) Core.Instance.Multiworld.EnableDeathLink();
                    else Core.Instance.Multiworld.DisableDeathLink();
                    StartCoroutine(UpdateOptionText());
                }
            }
        }

        public override void OnReleaseUp()
        {
            if (State == AppArchipelagoState.Options)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                CurrentOption = PrevOption;
                StartCoroutine(UpdateOptionText());
            }
        }

        public override void OnReleaseDown()
        {
            if (State == AppArchipelagoState.Options)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                CurrentOption = NextOption;
                StartCoroutine(UpdateOptionText());
            }
        }

        public override void OpenContent(AUnlockable unlockable, bool appAlreadyOpen)
        {
            if (State == AppArchipelagoState.Options) ChangeState(AppArchipelagoState.Chat);
            base.OpenContent(unlockable, appAlreadyOpen);
        }
    }
}
