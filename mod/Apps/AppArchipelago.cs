using Reptile;
using Reptile.Phone;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Archipelago.Structures;

namespace Archipelago.Apps
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
            StartingMovestyle,
            LimitedGraffiti,
            JunkPhotos,
            ScoreDifficulty,
            DamageMultiplier,
            DeathLink
        }

        public List<string> Messages
        {
            get { return Multiworld.messages; }
        }

        public AppArchipelagoState State => m_State;
        private AppArchipelagoState m_State = AppArchipelagoState.Chat;

        public AppArchipelagoOptions CurrentOption => m_CurrentOption;
        private static AppArchipelagoOptions m_CurrentOption = 0;

        public TextMeshProUGUI title;
        public TextMeshProUGUI text;
        public TextMeshProUGUI bottomLeftText;
        public UIButtonGlyphComponent bottomLeftGlyph;
        public TextMeshProUGUI bottomRightText;
        public UIButtonGlyphComponent bottomRightGlyph;
        public TextMeshProUGUI optionText;
        public UIButtonGlyphComponent upGlyph;
        public UIButtonGlyphComponent downGlyph;

        public static Color fadeColor = new Color(0.192f, 0.35f, 0.647f);
        public static Color highlightColor = new Color(0.882f, 0.953f, 0.345f);

        public void UpdateText()
        {
            text.text = string.Join("\n", Messages.ToArray());
        }

        internal void UpdateOptionText()
        {
            AppArchipelagoOptions prev1Option = m_CurrentOption - 1;
            AppArchipelagoOptions prev2Option = m_CurrentOption - 2;
            AppArchipelagoOptions next1Option = m_CurrentOption + 1;
            AppArchipelagoOptions next2Option = m_CurrentOption + 2;

            if ((int)prev1Option < 0) prev1Option = (AppArchipelagoOptions)(11 + (int)prev1Option);
            if ((int)prev2Option < 0) prev2Option = (AppArchipelagoOptions)(11 + (int)prev2Option);
            if ((int)next1Option > 10) next1Option = (AppArchipelagoOptions)((int)next1Option - 11);
            if ((int)next2Option > 10) next2Option = (AppArchipelagoOptions)((int)next2Option - 11);

            optionText.text = $"<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(prev2Option)}: {GetOptionValueText(prev2Option)}</color>\n"
                + $"<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(prev1Option)}: {GetOptionValueText(prev1Option)}</color>\n"
                + $"{GetOptionNameText(m_CurrentOption)}: <color=#{ColorUtility.ToHtmlStringRGBA(highlightColor)}>{GetOptionValueText(m_CurrentOption)}</color>"
                + $"\n<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(next1Option)}: {GetOptionValueText(next1Option)}</color>"
                + $"\n<color=#{ColorUtility.ToHtmlStringRGBA(fadeColor)}>{GetOptionNameText(next2Option)}: {GetOptionValueText(next2Option)}</color>";
        }

        private string GetOptionNameText(AppArchipelagoOptions option)
        {
            return option switch
            {
                AppArchipelagoOptions.Logic => Core.Instance.Localizer.GetRawTextValue("OPTION_LOGIC"),
                AppArchipelagoOptions.SkipIntro => Core.Instance.Localizer.GetRawTextValue("OPTION_SKIPINTRO"),
                AppArchipelagoOptions.SkipDreams => Core.Instance.Localizer.GetRawTextValue("OPTION_SKIPDREAMS"),
                AppArchipelagoOptions.SkipHands => Core.Instance.Localizer.GetRawTextValue("OPTION_SKIPHANDS"),
                AppArchipelagoOptions.TotalREP => Core.Instance.Localizer.GetRawTextValue("OPTION_TOTALREP"),
                AppArchipelagoOptions.StartingMovestyle => Core.Instance.Localizer.GetRawTextValue("OPTION_STARTINGMOVESTYLE"),
                AppArchipelagoOptions.LimitedGraffiti => Core.Instance.Localizer.GetRawTextValue("OPTION_LIMITEDGRAFFITI"),
                AppArchipelagoOptions.JunkPhotos => Core.Instance.Localizer.GetRawTextValue("OPTION_JUNKPHOTOS"),
                AppArchipelagoOptions.ScoreDifficulty => Core.Instance.Localizer.GetRawTextValue("OPTION_SCOREDIFFICULTY"),
                AppArchipelagoOptions.DamageMultiplier => Core.Instance.Localizer.GetRawTextValue("OPTION_DAMAGEMULTIPLIER"),
                AppArchipelagoOptions.DeathLink => Core.Instance.Localizer.GetRawTextValue("OPTION_DEATHLINK"),
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
                AppArchipelagoOptions.StartingMovestyle => ValueToText(Core.Instance.Data.startingMovestyle),
                AppArchipelagoOptions.LimitedGraffiti => ValueToText(Core.Instance.Data.limitedGraffiti),
                AppArchipelagoOptions.JunkPhotos => ValueToText(Core.Instance.Data.junkPhotos),
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
                if (boolValue) return Core.Instance.Localizer.GetRawTextValue("VALUE_BOOL_TRUE");
                else return Core.Instance.Localizer.GetRawTextValue("VALUE_BOOL_FALSE");
            }
            else if (value is Logic logicValue)
            {
                return logicValue switch
                {
                    Logic.Glitchless => Core.Instance.Localizer.GetRawTextValue("VALUE_LOGIC_GLITCHLESS"),
                    Logic.Glitched => Core.Instance.Localizer.GetRawTextValue("VALUE_LOGIC_GLITCHED"),
                    _ => "?"
                };
            }
            else if (value is TotalRep repValue)
            {
                return repValue switch
                {
                    TotalRep.Normal => Core.Instance.Localizer.GetRawTextValue("VALUE_REPVALUE_NORMAL"),
                    TotalRep.Less => Core.Instance.Localizer.GetRawTextValue("VALUE_REPVALUE_LESS"),
                    TotalRep.MuchLess => Core.Instance.Localizer.GetRawTextValue("VALUE_REPVALUE_MUCHLESS"),
                    _ => "?"
                };
            }
            else if (value is MoveStyle styleValue)
            {
                return styleValue switch
                {
                    MoveStyle.SKATEBOARD => Core.Instance.Localizer.GetRawTextValue("VALUE_MOVESTYLE_SKATEBOARD"),
                    MoveStyle.INLINE => Core.Instance.Localizer.GetRawTextValue("VALUE_MOVESTYLE_INLINE"),
                    MoveStyle.BMX => Core.Instance.Localizer.GetRawTextValue("VALUE_MOVESTYLE_BMX"),
                    _ => "?"
                };
            }
            else if (value is ScoreDifficulty scoreValue)
            {
                return scoreValue switch
                {
                    ScoreDifficulty.Normal => Core.Instance.Localizer.GetRawTextValue("VALUE_SCOREDIFFICULTY_NORMAL"),
                    ScoreDifficulty.Hard => Core.Instance.Localizer.GetRawTextValue("VALUE_SCOREDIFFICULTY_HARD"),
                    ScoreDifficulty.VeryHard => Core.Instance.Localizer.GetRawTextValue("VALUE_SCOREDIFFICULTY_VERYHARD"),
                    ScoreDifficulty.Extreme => Core.Instance.Localizer.GetRawTextValue("VALUE_SCOREDIFFICULTY_EXTREME"),
                    _ => "?"
                };
            }
            else if (value is int) return value.ToString();
            else return "?";
        }

        internal void UpdateGlyphs()
        {
            if (m_State == AppArchipelagoState.Chat)
            {
                bottomLeftText.text = Core.Instance.Localizer.GetRawTextValue("APP_NAVIGATION_CLOSE");
                bottomRightText.text = Core.Instance.Localizer.GetRawTextValue("APP_ARCHIPELAGO_NAVIGATION_OPTIONS");
                bottomRightText.gameObject.SetActive(true);
                bottomRightGlyph.gameObject.SetActive(true);
            }
            else if (m_State == AppArchipelagoState.Options)
            {
                bottomLeftText.text = Core.Instance.Localizer.GetRawTextValue("APP_NAVIGATION_BACK");
                if (m_CurrentOption == AppArchipelagoOptions.ScoreDifficulty || m_CurrentOption == AppArchipelagoOptions.DamageMultiplier || m_CurrentOption == AppArchipelagoOptions.DeathLink)
                {
                    bottomRightText.text = Core.Instance.Localizer.GetRawTextValue("APP_ARCHIPELAGO_NAVIGATION_CHANGE");
                    bottomRightText.gameObject.SetActive(true);
                    bottomRightGlyph.gameObject.SetActive(true);
                }
                else
                {
                    bottomRightText.gameObject.SetActive(false);
                    bottomRightGlyph.gameObject.SetActive(false);
                }
            }
        }

        internal void UpdateHeader()
        {
            if (m_State == AppArchipelagoState.Chat) title.text = Core.Instance.Localizer.GetRawTextValue("APP_ARCHIPELAGO_HEADER_DEFAULT");
            else if (m_State == AppArchipelagoState.Options) title.text = Core.Instance.Localizer.GetRawTextValue("APP_ARCHIPELAGO_HEADER_OPTIONS");
        }

        public void ChangeState(AppArchipelagoState state)
        {
            if (state == AppArchipelagoState.Chat)
            {
                m_State = AppArchipelagoState.Chat;
                UpdateHeader();
                UpdateGlyphs();
                text.gameObject.SetActive(true);
                optionText.gameObject.SetActive(false);
                upGlyph.gameObject.SetActive(false);
                downGlyph.gameObject.SetActive(false);
            }
            else if (state == AppArchipelagoState.Options)
            {
                m_State = AppArchipelagoState.Options;
                UpdateHeader();
                UpdateGlyphs();
                UpdateOptionText();
                text.gameObject.SetActive(false);
                optionText.gameObject.SetActive(true);
                upGlyph.gameObject.SetActive(true);
                downGlyph.gameObject.SetActive(true);
            }
        }

        public void ResizeDeltas()
        {
            Vector2 mainText = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.89f);
            Vector2 glyphs = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.94f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.88f);
            Vector2 inputTexts = new Vector2(transform.parent.GetComponent<RectMask2D>().canvasRect.width * 0.67f, transform.parent.GetComponent<RectMask2D>().canvasRect.height * 0.88f);
            text.GetComponent<RectTransform>().sizeDelta = mainText;
            optionText.GetComponent<RectTransform>().sizeDelta = mainText;
            bottomLeftGlyph.GetComponent<RectTransform>().sizeDelta = glyphs;
            bottomRightGlyph.GetComponent<RectTransform>().sizeDelta = glyphs;
            bottomLeftText.GetComponent<RectTransform>().sizeDelta = inputTexts;
            bottomRightText.GetComponent<RectTransform>().sizeDelta = inputTexts;
        }

        public void Start()
        {
            UpdateGlyphs();
            ResizeDeltas();
            Invoke("ResizeDeltas", 0.3f);
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

        public override void OnReleaseLeft()
        {
            if (m_State == AppArchipelagoState.Chat) MyPhone.CloseCurrentApp();
            else
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                ChangeState(AppArchipelagoState.Chat);
            }
        }

        public override void OnReleaseRight()
        {
            if (m_State == AppArchipelagoState.Chat)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                ChangeState(AppArchipelagoState.Options);
            }
            else if (m_State == AppArchipelagoState.Options)
            {
                if (m_CurrentOption == AppArchipelagoOptions.ScoreDifficulty)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.Instance.Data.scoreDifficulty += 1;
                    if (Core.Instance.Data.scoreDifficulty > ScoreDifficulty.Extreme) Core.Instance.Data.scoreDifficulty = ScoreDifficulty.Normal;
                    Core.Instance.stageManager.SetEncounterScores(WorldHandler.instance.SceneObjectsRegister);
                    UpdateOptionText();
                }
                else if (m_CurrentOption == AppArchipelagoOptions.DamageMultiplier)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.Instance.Data.damageMultiplier += 1;
                    if (Core.Instance.Data.damageMultiplier > 6) Core.Instance.Data.damageMultiplier = 1;
                    UpdateOptionText();
                }
                else if (m_CurrentOption == AppArchipelagoOptions.DeathLink)
                {
                    Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                    Core.Instance.Data.deathLink ^= true;
                    if (Core.Instance.Data.deathLink) Core.Instance.Multiworld.EnableDeathLink();
                    else Core.Instance.Multiworld.DisableDeathLink();
                    UpdateOptionText();
                }
            }
        }

        public override void OnReleaseUp()
        {
            if (m_State == AppArchipelagoState.Options)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                m_CurrentOption -= 1;
                if ((int)m_CurrentOption < 0) m_CurrentOption = AppArchipelagoOptions.DeathLink;

                UpdateOptionText();
                UpdateGlyphs();
            }
        }

        public override void OnReleaseDown()
        {
            if (m_State == AppArchipelagoState.Options)
            {
                Core.Instance.UIManager.PlaySfxGameplay(SfxCollectionID.PhoneSfx, AudioClipID.FlipPhone_Select);
                m_CurrentOption += 1;
                if ((int)m_CurrentOption > 10) m_CurrentOption = 0;

                UpdateOptionText();
                UpdateGlyphs();
            }
        }

        public override void OpenContent(AUnlockable unlockable, bool appAlreadyOpen)
        {
            if (m_State == AppArchipelagoState.Options) ChangeState(AppArchipelagoState.Chat);
            base.OpenContent(unlockable, appAlreadyOpen);
        }
    }
}
